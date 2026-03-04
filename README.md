В данном задании реализовал отдельными проектами: 
- Микросервис миграции БД (MigrationService)
- Фоновый сервис по обновлению валют (CurrencyBackgroundService)
- Микросервис управления валютами (FinanceService)
- Микросервис управления профилями (UserService)
- Микросервис Gateway (YARP на UserService и FinanceService)

Первым делом необходимо запустить сервис миграций. По умолчанию применит миграции для всех бд. 

Дальше можно запускать все другие сервисы.

CurrencyBackgroundService проверяет появились ли свежие данные у cbr.ru. Если еще нет, то следующий раз запросим через определенный промежуток времени. По умолчанию через 10 минут (можно поменять UpdateIntervalMinutes в appsettings). Сервис полезет в бд только в том случае, если появились новые данные. Поэтому мы не будем дергать нашу бд каждые 10 минут, а только раз в день при появлении новых данных).

Gateway проксирует запросы на отдельные микросервисы

UserService реализует следующие эндпойнты:
- [Post] http://localhost:5000/api/auth/register - регистрация нового пользователя (Будет выдан accessToken).
В Body надо прислать подобное: 
{"name":"string","password":"string"} 

- [Post] http://localhost:5000/api/auth/login - авторизация пользователя (Будет выдан accessToken).
В Body надо прислать подобное:
{"name":"string","password":"string"}

FinanceSerice реализует следующие эндпойнты:
- [Get] http://localhost:5000/api/currencies - получить список всех валют с курсом (доступно всем)

- [Get] http://localhost:5000/api/currencies/favorites - получить список избранных валют с курсом (необходим jwt-token)

- [Post] http://localhost:5000/api/currencies/favorites - добавить новую валюту в избранное (необходим jwt-token)
В Body надо прислать подобное:
{"CurrencyId":"1d0e3613-7abc-47af-b357-8477a7be335a"}

- [Delete] http://localhost:5000/api/currencies/favorites/1d0e3613-7abc-47af-b357-8477a7be335a - удалить определенную валюту из избранного (необходим jwt-token)


Эндпойнт для логаута не реализовывал, т.к. делал тестовое задание без брокера сообщений и без Redis. По хорошему, тут было бы два пути для решения это задачи:
1. Gateway делал бы запрос в UserService, если токен не был отозван. Если всё норм, то запрос шел бы дальше в нужный микросервис
2. Либо мы бы хранили список отозванных токенов в Redis. И тогда каждый микросервис мог бы сам проверить не был ли токен отозван (без лишнего запроса в UserService)

документацию сваггер можно получить по этому адресу
- http://localhost:5001/swagger/index.html - UserService
- http://localhost:5002/swagger/index.html - FinanceService

Gateway принимает запросы на 5000-ый порт

По дефолту названия бд такие:
- test-financedb для FinanceService
- test-userdb для UserService
