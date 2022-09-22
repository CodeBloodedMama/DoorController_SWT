using NSubstitute;
using NUnit.Framework;

namespace DoorControl.Test.Unit
{
    // Test Fixture for exception 1: Entry denied
    [TestFixture]
    public class DoorControlEntryDeniedTests
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

            // Ensure that validation will fail
            _userValidation.ValidateEntryRequest(Arg.Any<string>()).Returns(false);
        }


        [Test]
        public void RequestEntry_CardDbDeniesEntryRequest_DoorNotOpened()
        {
            _uut.RequestEntry("TFJ");
            _door.Received(0).Open();
        }
        [Test]
        public void RequestEntry_CardDbDeniesEntryRequest_BeeperMakeUnhappyNoiseCalled()
        {
            _uut.RequestEntry("TFJ");
            _entryNotification.Received().NotifyEntryDenied();
        }

        [Test]
        public void RequestEntry_CardDbDeniesEntryRequest_BeeperMakeHappyNoiseNotCalled()
        {
            _uut.RequestEntry("TFJ");
            _entryNotification.Received(0).NotifyEntryGranted();
        }

        [Test]
        public void RequestEntry_CardDbDeniesEntryRequest_AlarmNotSounded()
        {
            _uut.RequestEntry("TFJ");
            _alarm.Received(0).SoundAlarm();
        }

        [Test]
        public void RequestEntry_EntryDenied_ReadyForNormal()
        {
            _userValidation.ValidateEntryRequest("TFJ").Returns(true);

            // First a wrong entry
            _uut.RequestEntry("XXX");

            // Then we try a valid entry, we must be ready
            _uut.RequestEntry("TFJ");
            _door.Received(1).Open();

        }
    }
}