using CourseLibrary.Client;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    public class TestableClassWithApiAccessUnitTests
    {

        [Test]
        public void GetAuthors_On401Response_MustThrowUnauthorizedAccessExpection()
        {
            var httpClient = new HttpClient(new Return401UnauthorizedResponseHandler());
            var cancellationToken = new CancellationTokenSource();

            var testableClass = new TestableClassWithApiAccess(httpClient);
            AsyncTestDelegate act = () => testableClass.GetAuthors(cancellationToken.Token);

            Assert.That(act, Throws.TypeOf<UnauthorizedAccessException>());
            //Assert.ThrowsAsync<UnauthorizedAccessException>(() => response);
        }

        [Test]
        public void GetAuthors_On401Response_MustThrowUnauthorizedAccessExpection_WithMoq()
        {
            var unauthorizedResponseHttphandlerMock = new Mock<HttpMessageHandler>();

            unauthorizedResponseHttphandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                     ItExpr.IsAny<HttpRequestMessage>(),
                     ItExpr.IsAny<CancellationToken>()
                     ).ReturnsAsync(new HttpResponseMessage()
                     {
                         StatusCode = System.Net.HttpStatusCode.Unauthorized
                     });

            var httpClient = new HttpClient(unauthorizedResponseHttphandlerMock.Object);
            var testableClass = new TestableClassWithApiAccess(httpClient);
            var cancellationToken = new CancellationTokenSource();
            var r = testableClass.GetAuthors(cancellationToken.Token);
            AsyncTestDelegate act = () => testableClass.GetAuthors(cancellationToken.Token);

            Assert.That(act, Throws.TypeOf<UnauthorizedAccessException>());
            //Assert.ThrowsAsync<UnauthorizedAccessException>(() => response);
        }
    }
}