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
JobScheduler sets status for the job to something indicating it was considered, so that it isn't read again 
after pushing job request to a queue.

Handler deserializes payload and performs the job it knows how to perform.
Looking for ideas on jobs.
 - Generate PDF (from what?)
 - Send email
 - Compute pi to a given digit
 
## JobHandlers
These are bound to specific Job type. Inherit common abstract class, each will be hosted as dotnet BackgroundService.
(Can also host as Windows Services.) Each handler listens to its own queue.

If hosted in dockers
 docker build -t jobhandler.pdf ./PdfJobHandler
 docker run -d --name pdf-job jobhandler.pdf
 
In dev can be ran from command line as
 dotnet PdfJobHandler.dll
 
## JobRepositoryService
The only one interacting with MongoDB. Will contain endpoints for storing and reading job information.
### TODO: Repository service should contain domain logic (so not a pure repository anymore).
- I want to not allow setting status, if Final or Error status had been already set.
- I don't want allowing writing status in same case.


## Queueing
RabbitMQ will be used for communications. Most likely a docker image https://hub.docker.com/_/rabbitmq/

docker run -d --hostname my-rabbit --name some-rabbit  -p 5672:5672 -p 15672:15672 rabbitmq:3-management

## Logging
Perhaps another microservice. Where will it log?

# Notes
Downloading MongoDB. Done.
Launched MongoDB Compass. Created JobQueueSystem database and looking on what all this means. Apparently schema isn't created without data.
JobDocument added to Share project together with MongoDB.Driver NuGet.
Will need client API I guess for interacting with MongoDB. Separate service?

Appears I will factor in MasTrasit for rabbit.

# Tasks
- Create Repository service with MongoDB interaction. Good enough.
- Fetch Rabbit docker 
- Implement JobProducerService with MasTransit pushing JobRequest to the Rabbit.

# Ideas for Jobs
📄 DocIO (Word)

Create new Word documents from scratch.

Open and edit existing .doc / .docx.

Mail merge (populate Word templates with data).

Convert Word → PDF, HTML, RTF, TXT, EPUB.

Add bookmarks, tables, images, watermarks.

Protect documents with password & restrictions.

📊 XlsIO (Excel)

Create/edit Excel files (.xls, .xlsx).

Read/write formulas, pivot tables, charts.

Import/export data from DataSet, CSV, collections.

Apply styles, conditional formatting.

Convert Excel → PDF, CSV, HTML, Images.

Encrypt & protect worksheets/workbooks.

📑 PDF

Generate new PDF files.

Convert from Word/Excel/HTML → PDF.

Merge and split PDFs.

Add annotations, bookmarks, tables, images.

Apply digital signatures & certificates.

Compress PDFs (optimize size).

Protect with password & permissions.

🖼 Presentation (PowerPoint)

Create/edit PowerPoint (.pptx).

Add slides, images, charts, tables.

Convert PowerPoint → PDF or images.

Clone, merge, and split slide decks.

Apply themes, transitions, animations (basic).

📷 PDF-to-Image / Document-to-Image

Convert pages/slides/sheets → PNG/JPEG/TIFF.

Generate thumbnails & previews.

🔗 Other Utilities

HTML → PDF (with CSS/JavaScript support).

XML/JSON → Word/Excel/HTML via templates.

Barcode & QR code generation.

OCR (optional add-on).

⚡ In short:

Word/Excel/PDF/PowerPoint full CRUD.

Conversions between all of them.

Protection, signatures, compression.

Data binding (mail merge, DataSet import).

Images & preview rendering.
