using Moq;
using Portfolio.Data;
using Portfolio.Services.Interfaces;

namespace Portfolio.Services.Tests;

public class StockReportingServiceTests
{
    private Mock<IStockTickerService> _stockTickerServiceMock;
    private Mock<ITradeReaderService> _tradeReaderServiceMock;

    public StockReportingServiceTests()
    {
        _stockTickerServiceMock = new Mock<IStockTickerService>();
        _tradeReaderServiceMock = new Mock<ITradeReaderService>();
    }

    [Fact]
    public void GenerateReportWithItemRetrieval_ThrowsException_WhenNoTradesExist()
    {
        // Arrange
        _tradeReaderServiceMock.Setup(x => x.RetrieveItemsForPandL(It.IsAny<DateTime>())).Returns(new List<Trade>());

        var stockReportingService = new StockReportingService(_stockTickerServiceMock.Object, _tradeReaderServiceMock.Object);

        // Act
        // Assert
        Assert.ThrowsAsync<ApplicationException>(() => stockReportingService.GenerateReportWithItemRetrieval(DateTime.Today));
    }

    [Fact]
    public void GenerateReportWithItemRetrieval_ThrowsException_WhenStockInformationDoesNotExist()
    {
        // Arrange
        _tradeReaderServiceMock.Setup(x => x.RetrieveItemsForPandL(It.IsAny<DateTime>())).Returns(DataGenerator.GenerateTrades);
        _stockTickerServiceMock.Setup(x => x.GetStockInformationByDate(It.IsAny<string>(), It.IsAny<DateTime>())).ThrowsAsync(new ApplicationException());

        var stockReportingService = new StockReportingService(_stockTickerServiceMock.Object, _tradeReaderServiceMock.Object);

        // Act
        // Assert
        Assert.ThrowsAsync<ApplicationException>(() => stockReportingService.GenerateReportWithItemRetrieval(DateTime.Today));
    }

    [Fact]
    public void GenerateReportWithItemRetrieval_ReturnsValidReport()
    {
        // Arrange
        _tradeReaderServiceMock.Setup(x => x.RetrieveItemsForPandL(It.IsAny<DateTime>())).Returns(DataGenerator.GenerateTrades);
        _stockTickerServiceMock.Setup(x => x.GetStockInformationByDate(It.Is<string>(x => x == DataGenerator.GenerateTrades.ElementAt(0).Ticker), It.Is<DateTime>(x => x == DataGenerator.GenerateTrades.ElementAt(0).TradeDate))).ReturnsAsync(DataGenerator.StockInfoMocker);
        var stockReportingService = new StockReportingService(_stockTickerServiceMock.Object, _tradeReaderServiceMock.Object);

        // Act
        var reportBytes = stockReportingService.GenerateReportWithItemRetrieval(DateTime.Today);

        // Assert
        Assert.NotNull(reportBytes);
    }
}