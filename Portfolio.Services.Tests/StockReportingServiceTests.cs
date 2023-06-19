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

        // Act
        var stockReportingService = new StockReportingService(_stockTickerServiceMock.Object, _tradeReaderServiceMock.Object);

        // Assert
        Assert.ThrowsAsync<ApplicationException>(() => stockReportingService.GenerateReportWithItemRetrieval(DateTime.Today));
    }

    [Fact]
    public void GenerateReportWithItemRetrieval_ThrowsException_WhenStockInformationDoesNotExist()
    {
        // Arrange
        _tradeReaderServiceMock.Setup(x => x.RetrieveItemsForPandL(It.IsAny<DateTime>())).Returns(GenerateTrades);
        _stockTickerServiceMock.Setup(x => x.GetStockInformationByDate(It.IsAny<string>(), It.IsAny<DateTime>())).ThrowsAsync(new ApplicationException());

        // Act
        var stockReportingService = new StockReportingService(_stockTickerServiceMock.Object, _tradeReaderServiceMock.Object);

        // Assert
        Assert.ThrowsAsync<ApplicationException>(() => stockReportingService.GenerateReportWithItemRetrieval(DateTime.Today));
    }

    [Fact]
    public void GenerateReportWithItemRetrieval_ReturnsValidReport()
    {
        // Arrange
        _tradeReaderServiceMock.Setup(x => x.RetrieveItemsForPandL(It.IsAny<DateTime>())).Returns(GenerateTrades);
        _stockTickerServiceMock.Setup(x => x.GetStockInformationByDate(It.Is<string>(x => x == GenerateTrades.ElementAt(0).Ticker), It.Is<DateTime>(x => x == GenerateTrades.ElementAt(0).TradeDate))).ReturnsAsync(StockInfoMocker);

        // Act
        var stockReportingService = new StockReportingService(_stockTickerServiceMock.Object, _tradeReaderServiceMock.Object);

        // Assert
        var reportBytes = stockReportingService.GenerateReportWithItemRetrieval(DateTime.Today);

        Assert.NotNull(reportBytes);
    }

    private static List<Trade> GenerateTrades => new List<Trade> { new Trade("MSNBC", DateTime.Today, TradeAction.Buy, 100, 80.00m) };

    public static DailyStock? StockInfoMocker => new DailyStock
    {
        Price = 92.64m,
        Close = 90.77m,
        Date = new DateTime(2018, 04, 10)
    };
}
