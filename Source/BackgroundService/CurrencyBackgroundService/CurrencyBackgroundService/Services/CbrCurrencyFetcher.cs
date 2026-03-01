using System.Globalization;
using System.Text;
using System.Xml.Linq;
using CurrencyBackgroundService.Models;

namespace CurrencyBackgroundService.Services;

/// <summary>
/// Сервис для получения курсов валют из API ЦБ РФ
/// </summary>
public class CbrCurrencyFetcher(
    HttpClient httpClient, 
    ILogger<CbrCurrencyFetcher> logger)
{
    public async Task<CbrCurrenciesForCertainDay> FetchCurrenciesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Начинается загрузка курсов валют из ЦБ РФ");

            using var response = await httpClient.GetAsync("scripts/XML_daily.asp", cancellationToken);

            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var encoding = Encoding.GetEncoding("windows-1251");
            var xmlContent = encoding.GetString(bytes);
            var doc = XDocument.Parse(xmlContent);

            var date = ParseDate(doc);
            var currencies = ParseCurrencies(doc);
            
            logger.LogInformation("Успешно загружено {Count} валют из ЦБ РФ", currencies.Count);

            return new CbrCurrenciesForCertainDay
            {
                Date = date,
                Currencies = currencies
            };
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Ошибка HTTP при загрузке курсов валют");
            throw;
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(ex, "Таймаут при загрузке курсов валют");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Неожиданная ошибка при загрузке курсов валют");
            throw;
        }
    }

    /// <summary>
    /// Парсит дату документа
    /// </summary>
    private DateOnly ParseDate(XDocument doc)
    {
        var dateRaw = doc.Root?.Attribute("Date")?.Value;
        var cbrDate = DateOnly.ParseExact(dateRaw!, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        return cbrDate;
    }

    /// <summary>
    /// Парсит XML ответ от ЦБ РФ
    /// </summary>
    private List<CbrCurrencyDto> ParseCurrencies(XDocument doc)
    {
        try
        {
            var currencies = new List<CbrCurrencyDto>();

            foreach (var valute in doc.Descendants("Valute"))
            {
                try
                {
                    var currency = new CbrCurrencyDto
                    {
                        NumCode = valute.Element("NumCode")?.Value ?? string.Empty,
                        CharCode = valute.Element("CharCode")?.Value ?? string.Empty,
                        Nominal = int.Parse(valute.Element("Nominal")?.Value ?? "0"),
                        Name = valute.Element("Name")?.Value ?? string.Empty,
                        Value = ParseDecimal(valute.Element("Value")?.Value ?? "0"),
                        VunitRate = ParseDecimal(valute.Element("VunitRate")?.Value ?? "0"),
                    };
                    
                    // Пропускаем валюты с некорректными данными
                    if (string.IsNullOrEmpty(currency.CharCode) || string.IsNullOrEmpty(currency.Name) ||
                        currency.Nominal <= 0 || currency.VunitRate <= 0)
                    {
                        logger.LogWarning("Пропущена валюта с некорректными данными: {CharCode}", currency.CharCode);
                        continue;
                    }

                    currencies.Add(currency);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Ошибка при парсинге валюты");
                }
            }

            return currencies;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при парсинге XML");
            throw;
        }
    }

    private static decimal ParseDecimal(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new FormatException("Пустое значение decimal");

        // Нормализуем десятичный разделитель на случай "77,7335"
        var normalized = value.Trim().Replace(',', '.');

        if (decimal.TryParse(
                normalized,
                NumberStyles.Float, // важно: поддерживает E-notation
                CultureInfo.InvariantCulture,
                out var result))
        {
            return result;
        }

        throw new FormatException($"Не удалось распарсить decimal: '{value}'");
    }
}