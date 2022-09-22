using System;
using NSubstitute;
using NUnit.Framework;

namespace DoorControl.Test.Unit
{
    // Test Fixture for the main (sunny-day) scenario
    [TestFixture]
    public class DoorControlEntryGrantedTests
    {
        private DoorControl _uut;
        private IUserValidation _userValidation;
        private IDoor _door;
        private IEntryNotification _entryNotification;
        private IAlarm _alarm;

        [SetUp]
        public void Setup()
        {
            _userValidation = Substitute.For<IUserValidation>();
            _door = Substitute.For<IDoor>();
            _entryNotification = Substitute.For<IEntryNotification>();
            _alarm = Substitute.For<IAlarm>();

            _uut = new DoorControl(_userValidation, _door, _entryNotification, _alarm);
            // Setup for user TFJ to be allowed
            _userValidation.ValidateEntryRequest("TFJ").Returns(true);

        }

        [Test]
        public void RequestEntry_CorrectIdUsedForDbQuery()
        {
            _uut.RequestEntry("TFJ");
            _userValidation.Received(1).ValidateEntryRequest(("TFJ"));
        }

        [Test]
        public void RequestEntry_CardDbApprovesEntryRequest_DoorOpenCalled()
        {
            _uut.RequestEntry("TFJ");
            _door.Received(1).Open();
        }

        [Test]
        public void RequestEntry_CardDbApprovesEntryRequest_DoorCloseNotCalled()
        {
            _uut.RequestEntry("TFJ");
            _door.Received(0).Close();
        }

        [Test]
        public void RequestEntry_CardDbApprovesEntryRequest_BeeperMakeHappyNoiseCalled()
        {
            _uut.RequestEntry("TFJ");
            _entryNotification.Received(1).NotifyEntryGranted();
        }

        [Test]
        public void RequestEntry_CardDbApprovesEntryRequest_BeeperMakeUnhappyNoiseNotCalled()
        {
            _uut.RequestEntry("TFJ");
            _entryNotification.Received(0).NotifyEntryDenied();
        }


        [Test]
        public void RequestEntry_DoorOpened_DoorIsClosed()
        {
            _uut.RequestEntry("TFJ");
            _uut.DoorOpened();
            _door.Received(1).Close();
        }



        [Test]
        public void RequestEntry_DoorOpenedAndClosed_AlarmNotSounded()
        {
            _uut.RequestEntry("TFJ");
            _uut.DoorOpened();
            _uut.DoorClosed();
            _alarm.Received(0).SoundAlarm();
       }

        [Test]
        public void RequestEntry_DoorNotYetClosedAgain_NoAction()
        {
            _uut.RequestEntry("TFJ");
            _uut.DoorOpened();

            _userValidation.ClearReceivedCalls();

            _uut.RequestEntry("TFJ");

            // We should not react on this, until we are closed again
            _userValidation.DidNotReceive().ValidateEntryRequest("TFJ");


        }

        [Test]
        public void RequestEntry_FullCycle_CanRestart()
        {
            _uut.RequestEntry("TFJ");
            _uut.DoorOpened();
            _uut.DoorClosed();

            // We should now be ready for a new opening
            _door.ClearReceivedCalls();

            _uut.RequestEntry("TFJ");
            _door.Received(1).Open();
        }

    }
}
