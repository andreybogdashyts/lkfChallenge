using Lkf.Migration.Infrastructure.DataManager;
using Lkf.Migration.Infrastructure.Processors;
using Lkf.Migration.Infrastructure.SearchEngine;
using Lkf.Migration.Models;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lkf.Migration.Tests.Infrastructure.Processors
{
    [TestClass]
    public class MigrationProcessTests
    {
        private Mock<ISearchEngine> _searchEngineMock;
        private Mock<IDataManager> _dataManagerMock;
        private List<BatchOptions> _batchOptions;
        private List<Collection> _collections;

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            _searchEngineMock = new Mock<ISearchEngine>();
            _dataManagerMock = new Mock<IDataManager>();
            _batchOptions = new List<BatchOptions>
            {
                new BatchOptions
                {
                    Skip = 0,
                    TakeCount = 10,
                },
                new BatchOptions
                {
                    Skip = 10,
                    TakeCount = 10,
                }
            };
            _collections = new List<Collection>
            {
                new Collection{ Id = "1" },
                new Collection{ Id = "2" },
            };
        }

        #endregion

        [TestMethod]
        public void TestMigrateAsync()
        {
            _searchEngineMock.Setup(m => m.CollectionExists(It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            _searchEngineMock.Setup(m => m.SendBatch(It.IsAny<string>(), It.IsAny<List<Collection>>()))
                .Returns(Task.FromResult(true));

            _dataManagerMock.SetupProperty(m => m.BatchOptions, _batchOptions);
            _dataManagerMock.Setup(m => m.GetCollections(It.IsAny<BatchOptions>()))
                .Returns(_collections);

            var mp = new MigrationProcess(_searchEngineMock.Object, _dataManagerMock.Object);
            var r = mp.MigrateAsync().Result;

            Assert.IsTrue(r);
            //....
        }

        [TestMethod]
        public void TestMigrateAsync_FailSend()
        {
            _searchEngineMock.Setup(m => m.CollectionExists(It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            _searchEngineMock.Setup(m => m.SendBatch(It.IsAny<string>(), It.IsAny<List<Collection>>()))
                .Returns(Task.FromResult(false));

            _dataManagerMock.SetupProperty(m => m.BatchOptions, _batchOptions);
            _dataManagerMock.Setup(m => m.GetCollections(It.IsAny<BatchOptions>()))
                .Returns(_collections);

            var mp = new MigrationProcess(_searchEngineMock.Object, _dataManagerMock.Object);
            var r = mp.MigrateAsync().Result;

            Assert.IsFalse(r);
            //....
        }
    }
}
