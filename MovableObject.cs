using System.Xml.Serialization;

namespace FLOW.NET
{
    [XmlType("MovableObject")]
    public abstract class MovableObject : FLOWObject
    {
        private double entryTime;
        private bool inTransfer;
        private bool isBlocked;
        private bool isJammed;
        private FLOWObject location;

        public MovableObject()
        {
        }

        public MovableObject(string nameIn, FLOWObject parentIn)
            : base(nameIn, parentIn)
        {
        }

        #region GETSET functions
        [XmlIgnore()]
        public double EntryTime
        {
            get { return this.entryTime; }
            set { this.entryTime = value; }
        }

        [XmlIgnore()]
        public bool InTransfer
        {
            get { return this.inTransfer; }
            set { this.inTransfer = value; }
        }

        [XmlIgnore()]
        public bool IsBlocked
        {
            get { return this.isBlocked; }
            set { this.isBlocked = value; }
        }

        public FLOWObject Location
        {
            get { return this.location; }
            set { this.location = value; }
        }
        #endregion

        public virtual void ChangeLocation(double timeIn, FLOWObject locationIn)
        {
        }
    }

    [XmlType("MovableObjectState")]
    public class MovableObjectState : FLOWObjectState
    {
        private double entryTime;
        private bool inTransfer;
        private bool isBlocked;

        public MovableObjectState()
        {
        }

        #region GETSET functions
        [XmlElement("EntryTime")]
        public double EntryTime
        {
            get { return this.entryTime; }
            set { this.entryTime = value; }
        }

        [XmlElement("InTransfer")]
        public bool InTransfer
        {
            get { return this.inTransfer; }
            set { this.inTransfer = value; }
        }

        [XmlElement("IsBlocked")]
        public bool IsBlocked
        {
            get { return this.isBlocked; }
            set { this.isBlocked = value; }
        }

        #endregion

        public void GetState(MovableObject movableObjectIn)
        {
            base.GetState(movableObjectIn);
            this.entryTime = movableObjectIn.EntryTime;
            this.inTransfer = movableObjectIn.InTransfer;
            this.isBlocked = movableObjectIn.IsBlocked;
        }

        public void SetState(MovableObject movableObjectIn)
        {
            movableObjectIn.EntryTime = this.entryTime;
            movableObjectIn.InTransfer = this.inTransfer;
            movableObjectIn.IsBlocked = this.isBlocked;
            base.SetState(movableObjectIn);
        }
    }
}