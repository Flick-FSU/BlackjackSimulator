using BlackjackSimulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackjackSimulatorTest
{
    public class ScenarioBuilderTest
    {
        private ScenarioBuilder _sut;

        [TestInitialize]
        public void MyTestInitialize()
        {
            _sut = new ScenarioBuilder();
        }

    }
}