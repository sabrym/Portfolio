using System;
using DocumentFormat.OpenXml.Drawing.Charts;
using Moq;
using Portfolio.Data.Configs;
using Portfolio.Services.Interfaces;
using Portfolio.Utilities;

namespace Portfolio.Services.Tests
{
	public class XMLTradeReaderServiceTests
	{
        private readonly Mock<IFileWrapper> _fileWrapperMock;

        public XMLTradeReaderServiceTests()
		{
			_fileWrapperMock = new Mock<IFileWrapper>();
		}

		[Fact]
		public void RetrieveItemsForPandLWhenNoItemsInDirectory_ThrowsException()
		{
			// arrange
			var configuration = new TradesConfig
			{
				Directory = "SomeDirectory",
				DocumentTag = "tag",
				MonthYearDelimiter = "mm-YYYY"
			};

			_fileWrapperMock.Setup(x => x.DirectoryIsEmpty(configuration.Directory)).Returns(true);

			var xmlTradeReaderService = new XMLTradeReaderService(configuration, _fileWrapperMock.Object);

            // act
            // assert
            Assert.Throws<Utilities.ApplicationException>(() => xmlTradeReaderService.RetrieveItemsForPandL(DateTime.Today));
        }

        [Fact]
        public void RetrieveItemsWhenInvalidDataExists_ThrowsException()
        {
            // arrange
            var configuration = new TradesConfig
            {
                Directory = "SomeDirectory",
                DocumentTag = "tags",
                MonthYearDelimiter = "mm-YYYY"
            };

            _fileWrapperMock.Setup(x => x.DirectoryIsEmpty(configuration.Directory)).Returns(true);

            var xmlTradeReaderService = new XMLTradeReaderService(configuration, _fileWrapperMock.Object);

            // act
            // assert
            Assert.Throws<Utilities.ApplicationException>(() => xmlTradeReaderService.RetrieveItemsForPandL(DateTime.Today));
        }

        [Fact]
        public void RetrieveItemsWithValidItemsInDirectory_ReturnsValidResult()
        {
            // arrange
            var configuration = new TradesConfig
            {
                Directory = "../../../TestFiles/",
                DocumentTag = "trade",
                MonthYearDelimiter = "MM-yyyy",
                FileName = "Month-{0}.xml"
            };

            var xmlTradeReaderService = new XMLTradeReaderService(configuration, new FileWrapper());

            // act
            var result = xmlTradeReaderService.RetrieveItemsForPandL(new DateTime(2018, 04, 10));

            // assert
            Assert.NotEmpty(result);
        }
    }
}

