using System;
using System.Collections.Generic;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class SimulationsRunnerTest
    {
        [TestMethod] //todo: fails due to multithreading?
        [Ignore]
        public void When_Running_Two_Simulations_Should_Call_Table_Simulation_Run_Method_Twice()
        {
            var sut = new SimulationsRunner();
            var strictMockTableSimulation = MockRepository.GenerateStrictMock<ITableSimulation>();
            strictMockTableSimulation.Expect(mts => mts.NumberOfPlayers).Repeat.Twice().Return(1);
            strictMockTableSimulation.Expect(mts => mts.RunSimulationUntilAllPlayersUnregister()).Repeat.Twice()
                .Return(new List<IPlayer>());

            strictMockTableSimulation.Replay();

            sut.Load(strictMockTableSimulation);
            sut.Run(2);

            strictMockTableSimulation.VerifyAllExpectations();
        }
    }
}
