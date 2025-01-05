# <p style="text-align: center;">Распределенный сервис постановки и выполнения абстрактных задач с контролем времени жизни процесса</p>
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/Rabbitmq-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![Redis](https://img.shields.io/badge/redis-%23DD0031.svg?style=for-the-badge&logo=redis&logoColor=white)
![Elasticsearch](https://img.shields.io/badge/elasticsearch-%230377CC.svg?style=for-the-badge&logo=elasticsearch&logoColor=white)
![Kibana](https://img.shields.io/badge/Kibana-005571?style=for-the-badge&logo=Kibana&logoColor=white)
![Kubernetes](https://img.shields.io/badge/kubernetes-%23326ce5.svg?style=for-the-badge&logo=kubernetes&logoColor=white)
![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white)
![Postgres](https://img.shields.io/badge/postgres-%23316192.svg?style=for-the-badge&logo=postgresql&logoColor=white)

Сервис для постановки, выполнения и мониторинга абстрактных задач с возможностью контроля времени жизни процесса (TTL, Time-to-Live).

## Использование

Сервисы доступны по адресам:
1. ApiGateway -http://84.252.135.174/swagger/index.html
2. Service Discovery - http://84.252.135.174/sd/swagger/index.html
3. Users - http://84.252.135.174/users/swagger/index.html
4. Abstract task service -http://84.252.135.174/tasks/swagger/index.html
5. Elasticsearch - http://84.252.135.174:9200/
6. Kibana - http://84.252.135.174:5601/

## Описание

 ### Архитектура
 Ответственности сервисов можно увидеть на схеме:
 ![image](https://github.com/user-attachments/assets/1d19f855-7d04-46cf-8a61-10323bf7c103)

 ### Код
 Выделили проект `Infrastructure`, чтобы стандартизировать общие моменты сервисов + упростить заведение новых API и Client проектов.
 Сделали свой `Service Discovery` в качестве интереса.
 Сделали авторизацию/регистрацию - сервис `Users`, но не успели закрыть ею endpoint-ы.
 Основа проекта - `AbstracttaskService` и `AbstractTaskWorker`.
 Захостили сервис в docker-compose на ВМ в сервисе yandex cloud.

 Добавлено `CD` с использованием `github.actions`

 ### Постановка задач:
 Сервис принемает задачи от внешних систем через API. 
 Задача содержит описание задачи и максимальное время на выполнение (TTL).
 
 ### Очередь задач:
 Задачи добавляются в очередь брокера сообщений RabbitMQ.
 
 ### Выполнение задач:
 Обработка задач выполняется отдельным сервисом - Worker. Он прослушивает очередь задач, кэширует в Redis выполняемые задачи для отслеживания процесса их выполнения. 
 Если TTL истекает или задача выполняется успешно, задача удаляется из кэша и сохраняется в базу данных. 
 Обработка задач распределена между несколькими экземплярами
 сервиса для обеспечения отказоустойчивости и высокой доступности.
 
 ### Мониторинг и контроль:
 Сервис предоставляет API для мониторинга состояния задач и 
 возможность повторного выполнения задач по запросу пользователя.

 ### Протокол взаимодействия:
 Взаимодействие с внешними системами реализовано через REST API
 
 ### Безопасность:
 Сейчас, для наглядности, доступны swagger-ы всех сервисов. В реальности стоило бы открыть только ApiGateway и закрыть его https соединением. 
 
 ### Логирование и мониторинг:
 Каждое приложение пишет логи в 3 места:
 1. Консоль
 2. Локальный файл - в swagger каждоого сервиса есть Endpoint, где можно посмотреть логи
 3. В Elasticsearch
