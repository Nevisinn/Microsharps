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

Api доступно по адресу ...

## Описание

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
 ### Архитектура:

 ### Протокол взаимодействия:
 Взаимодействие с внешними системами реализовано через REST API
 ### Безопасность:
 ### Логирование и мониторинг: