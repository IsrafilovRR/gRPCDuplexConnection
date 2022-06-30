# WeatherSystem.EventClient and WeatherSystem.EventGenerator

## Requirements
1. Сервис-эмулятор погодных датчиков.
	Сервис должен генерировать события от минимум двух погодных датчиков (можно больше) уличного и датчика внутри помещения
	В событии от датчиков должны быть данные по текущей температуре, влажности и содержанию CO2. Желательно учесть, что уровень CO2 на уличном датчике обычно +- одинаковый, в отличии от датчиков, находящихся в помещениях.
	Должен реализовывать Grpc-сервис, который в потоковом режиме будет возвращать события из датчиков.
	Должен реализовывать REST-метод, который возвращает текущие параметры каждого датчика. Например, сейчас нужно прямо узнать какие последние данные были на датчике.
	Интервал генерации событий вынести в настройки.
	Интервал не должен превышать 2 секунд. Т.е. события должны генерироваться хотя бы одно в 2 секунды по каждому датчику.
	Не стоит выставлять и слишком маленький интервал - минимальное значение 100мс.
	
2. Сервис-клиент обработки событий от сервиса-эмулятора погодных датчиков
Должен уметь подписывается на получение данных от конкретного датчика или группы датчиков.
Должен уметь отписываться от получения информации по одному или всем датчикам.
Должен взаимодействовать с сервисом-эмулятором через полнодуплексный grpc stream.
Должен уметь переподнимать поток, если вдруг происходит разрыв связи. Например, если сервис-эмулятор остановлен, то необходимо пробовать подключаться с нему, до победного. Плюсом будет использование более сложного алгоритма ожидания, чем простой Delay.
	
Должен оперативно аггрегировать информацию в следующих разрезах:
	1. Средняя температура по каждому датчику за 1 минуту.
	2. Средняя влажность по каждому датчику за 1 минуту.
	3. Максимальное и минимальное содержание CO2 по каждому датчику за 1 минуту.
		
Должен иметь ручки, для чтения агрегированных данных в разрезе указанного интервала. Например, я хочу получить среднюю температуру за 10 минут начиная с 13:44. Значит ручка должна вытащить уже сагрегированные данные по 1 минуте, и посчитать из них среднюю за 10 минут.
Должен иметь настройку для изменения интервала аггрегации. Если нужно аггрегировать не за 1 минуту, а за 10 минут.
Должен иметь ручку для диагностики, которая выводит все сохраненные данные по каждому датчику.

## Description
There is gRPC duplex connection between EventClient and EventGenerator.
EventGenerator generate events and can change them based on events from EventGenerator

# WeatherSystem.Common.RateLimiter

## Requirements
1. Необходимо реализовать rate-limiter (алгоритм fixed window) для web api в .net core приложении. 
Настройки:
* количество одновременных запросов
* период времени
задаются глобально для всего приложения, при этом есть возможность:
* переопределить параметры на методах в контроллере;
* задать индивидуальные настройки для потребителей.
Клиенты идентифицируются по IP.
В случае превышения лимита, клиент должен получить HTTP ошибку 429.
2. *** Настройки лимитов желательно хранить не в памяти сервиса, а в отдельном внешнем хранилище (например, в БД). В случае внешнего хранения не забыть про то, что внешние системы бывают недоступны и это не повод для сервиса работать совсем без каких-либо лимитов.

## Types of request limiters

You can specify or use several types of request limiter in time window:
- Use client individual repository and cache.
- Use attribute RequestLimitsAttribute on an endpoint
  (see the example: EventClient.Controllers.AggregationController )
- Use global settings, which is configured in Config file

### Individual request limits

We have the cache of individual request limits, which is updated periodically by ClientLimitsCacheUpdaterHostedService/
Hosted service take data from repository.

TODO: Repository is stubbed, we have to implement real DB.

## Priority of the limits

What is priority of that limits:
- Highest priority is set for individual client request limits from Cache or Repository.
- Second priority is set for separate endpoints
- And the last one - is global from config.

## How it works

All requests go through RequestLimiterMiddleware. At first step we should take a request limits.

A client, which has individual limits, does the request to the endpoint, that has its 
own request limits too. Here we take individual limits, because it is more priority. 

Is we don't have any special limits, like individual or endpoint, then we take global limits.

Then we send taken limit, ip address and endpoint (if it has own limits) to  ILimitsRequestCheckerService.

# WeatherSystem.Common.DataAccess

## Requirements
Необходимо создать сервис с подключенной БД.

Вся работа с БД должна осуществляться через dapper
Сервис должен уметь сохранять массивы заказов. Заказ состоит из следующих атрибутов

- Id - guid
- Идентификатор клиента(long)
- Дата создания заказа (datetime)
- Дата Выдачи заказа (datetime)
- Статус заказа (New, InProgress, Pending)
- Список товаров - (Массив из товаров(Id товара(long), Колличество(int)))
- Id Склада отгрузки (long)

Заказы нужно сохранять в партиционированную таблицу(партиционирование по складу отгрузки)

Должен быть реализован метод умеющий искать заказы по складу отгрузки, статусу и диапазону дат

Метод поиска должен возвращать результаты в виде потока данных(то есть нужно уметь читать данные частями через итератор, а не все сразу)

Скрипт создания схемы бд должен быть просто в виде sql файла, не нужно делать механизм накатки миграций

## Description
This is the library that contains repositories with connection to postgresql database.

## Init

Run scripts/init_database.sql script

## OrderRepository 
OrderRepository has two methods:

- GetOrdersAsync - returns orders as async enumerable
- SaveOrdersAsync - do bulk insert to db
