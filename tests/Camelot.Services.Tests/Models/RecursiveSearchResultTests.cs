using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.RecursiveSearch;
using Camelot.Services.Models;
using Xunit;

namespace Camelot.Services.Tests.Models
{
    public class RecursiveSearchResultTests
    {
        private const string NodePath = "NodePath";

        [Fact]
        public async Task TestTask()
        {
            var isTaskCalled = false;
            Task TaskFactory(INodeFoundEventPublisher r)
            {
                isTaskCalled = r != null;

                return Task.CompletedTask;
            }

            var result = new RecursiveSearchResult(TaskFactory);

            await result.Task.Value;

            Assert.True(isTaskCalled);
        }

        [Fact]
        public void TestEvent()
        {
            static Task TaskFactory(INodeFoundEventPublisher r) => Task.CompletedTask;

            var isCallbackCalled = false;

            var result = new RecursiveSearchResult(TaskFactory);
            result.NodeFoundEvent += (sender, args) => isCallbackCalled = args.NodePath == NodePath;

            result.RaiseNodeFoundEvent(NodePath);

            Assert.True(isCallbackCalled);
        }
    }
}