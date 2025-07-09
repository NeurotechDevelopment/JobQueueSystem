# JobQueueSystem
Exercise to brush up on microservices, distributed architecture.
Gain familiarity working with MongoDB, RabbitMQ, MediatR.
Exercise good patterns.

System accepts job requests. Jobs are queued and stored.
Job scheduler queries pending jobs and schedules with appropriate handlers.
Service for querying jobs and their results will be made available.

Later everything can be dockerized.

## Database
MongoDB

JobRequest: JobId, Status, Payload, ReceivedAt (UTC)
JobResult: JobId, Result

## JobProducerService
.NET Core WebApi exposing POST method for submitting job requests.
It will push such requests to the queue.
JobId guid is to be assigned by a caller.

## JobReceiverService
Subscribed to queue for incoming job requests. Creates new entry in database
with JobId, Status = 'Received', Payload, ReceivedAt = UTC Now.

## JobScheduler Service
To be decided what stack will be used.
Presumably it will read Job type and with MediatR resolve the appropriate handler.
Perhaps each handler will be a separate microservice.
Handler deserializes payload and performs the job it knows how to perform.
Looking for ideas on jobs.
 - Generate PDF (from what?)
 - Send email
 - Compute pi to a given digit
 
## JobRepositoryService
The only one interacting with MongoDB. Will contain endpoints for storing and reading job information.

## Queueing
RabbitMQ will be used for communications. Most likely a docker image https://hub.docker.com/_/rabbitmq/

## Logging
Perhaps another microservice. Where will it log?

# Progress
Downloading MongoDB. Done.
Launched MongoDB Compass. Created JobQueueSystem database and looking on what all this means. Apparently schema isn't created without data.
JobDocument added to Share project together with MongoDB.Driver NuGet.
Will need client API I guess for interacting with MongoDB. Separate service?

Appears I will factor in MasTrasit for rabbit.
