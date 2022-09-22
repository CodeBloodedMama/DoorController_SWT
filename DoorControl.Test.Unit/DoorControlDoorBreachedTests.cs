using NSubstitute;
using NUnit.Framework;

namespace DoorControl.Test.Unit
{
    // Test Fixture for exception 2: Door breached (i.e. opened without prior validation)
    [TestFixture]
    public class DoorControlDoorBreachedTests
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
        }

        [Test]
        public void DoorBreached_DoorStateIsBreached_AlarmCalled()
        {
            _uut.DoorOpened();  // Breach door
            // NSubstitute will give you the mocked object, when calling the create method again!
            _alarm.Received(1).SoundAlarm();
        }

        [Test]
        public void DoorBreached_DoorStateIsBreached_CloseDoorCalled()
        {
            _uut.DoorOpened();  // Breach door
            _door.Received().Close();
        }
    }
}