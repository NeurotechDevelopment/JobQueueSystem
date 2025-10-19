using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace JobRepositoryService
{
    public class GridFsBlobStorage : IBlobStorage
    {
        private const string BucketName = "JobsRepositoryBucket";
        private readonly GridFSBucket gridFsBucket;

        public GridFsBlobStorage(IMongoClient client, IOptions<ApplicationSettings> optionSettings)
        {
            var db = client.GetDatabase(optionSettings.Value.Database);
            this.gridFsBucket = new GridFSBucket(db, new GridFSBucketOptions
            {
                BucketName = BucketName
            });
        }

        public async Task<string> GetFileIdByTagNameAsync(string tag)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Metadata["tag"].AsString, tag);
            var asyncFileCursor = await this.gridFsBucket.FindAsync(filter);
            var file = await asyncFileCursor.FirstOrDefaultAsync();
            return file?.Id.ToString();
        }

        public async Task<(string tag, string fileName, string contentType)> GetFileInfoAsync(string id)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
            var asyncFileCursor = await this.gridFsBucket.FindAsync(filter);
            var file = await asyncFileCursor.FirstOrDefaultAsync();
            file.Metadata.TryGetValue("tag", out var tag);
            file.Metadata.TryGetValue("contentType", out var contentType);
            return (tag.AsString, file.Filename, contentType.AsString);
        }

        public async Task<bool> FileExistsAsync(string id)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
            var asyncFileCursor = await this.gridFsBucket.FindAsync(filter);
            return await asyncFileCursor.AnyAsync();
        }

        public async Task<string> UploadAsync(string tag, string blobName, string contentType, byte[] data)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument
                {
                    { "tag", tag },
                    { "blobName", blobName },
                    { "contentType", contentType }
                }
            };

            using (var stream = await this.gridFsBucket.OpenUploadStreamAsync(blobName, options))
            {
                var id = stream.Id.ToString();
                await stream.WriteAsync(data, 0, data.Length);
                await stream.CloseAsync();
                return id;
            }
        }

        public async Task<string> UploadStreamAsync(string tag, string blobName, string contentType, Stream data)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument
                {
                    { "tag", tag },
                    { "blobName", blobName },
                    { "contentType", contentType }
                }
            };
            var id = await this.gridFsBucket.UploadFromStreamAsync(blobName, data, options);
            return id.ToString();
        }

        public async Task<byte[]> DownloadAsync(string id)
        {
            return await this.gridFsBucket.DownloadAsBytesAsync(ObjectId.Parse(id));
        }

        public async Task<Stream> DownloadStreamAsync(string id)
        {
            var stream = new MemoryStream();
            await this.gridFsBucket.DownloadToStreamAsync(ObjectId.Parse(id), stream);
            stream.Position = 0; // Reset the stream position to the beginning
            return stream;
        }

        public async Task DeleteAsync(string id)
        {
            /*
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
            var asyncFileCursor = await this.gridFsBucket.FindAsync(filter);
            var file = await asyncFileCursor.FirstOrDefaultAsync();
            */
            await this.gridFsBucket.DeleteAsync(ObjectId.Parse(id));
        }
    }
}
