using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Users.Managers
{
    public class UserInfoBatchCsvFileProcessor : IFileProcessor
    {
        private readonly string fileHeaderOrder = "first_name,last_name,age,date,country,province,city";
        private readonly IDataHandler<User> _dataHandler;

        public UserInfoBatchCsvFileProcessor(IDataHandler<User> dataHandler) {
            _dataHandler = dataHandler;
        }

        public async Task<bool> Process(Stream stream)
        {
            try
            {
                await ValidateFileHeaderOrderFrom(stream);
                stream.Position = fileHeaderOrder.Length + 1;
                var result = await _dataHandler.BulkUserDataFromFile(stream);

                return result;
            } catch (Exception ex)
            {
                throw new AppException(
                    "Something went wrong when saving the info",
                    new[] { ex.Message });
            }
        }

        private async Task ValidateFileHeaderOrderFrom(Stream stream)
        {
            var streamReader = new StreamReader(stream);
            var line = await streamReader.ReadLineAsync();
            if (line?.Trim() != fileHeaderOrder)
            {
                throw new AppException($"File header is not valid. Order expected: {fileHeaderOrder}");
            }

            int lineCount = 0;
            while (streamReader.ReadLine() != null)
            {
                lineCount++;
            }

            if (lineCount > 1000)
            {
                throw new AppException($"File has more than 1000 lines");
            }
        }
    }
}
