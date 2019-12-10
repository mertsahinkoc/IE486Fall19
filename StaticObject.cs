using System.Xml.Serialization;

namespace FLOW.NET
{
    [XmlType("StaticObject")]
    public abstract class StaticObject : FLOWObject
    {
        private int capacity;
        private MovableObjectList content;
        private bool isBlocked;
        private bool isStarved;     //IE486f18
        private int reserved;

        public StaticObject()
        {
            this.content = new MovableObjectList();
        }

        public StaticObject(string nameIn, FLOWObject parentIn, int capacityIn)
            : base(nameIn, parentIn)
        {
            this.capacity = capacityIn;
            this.content = new MovableObjectList();
        }

        #region GETSET functions
        [XmlElement("Capacity")]
        public int Capacity
        {
            get { return this.capacity; }
            set { this.capacity = value; }
        }

        [XmlIgnore()]
        public MovableObjectList Content
        {
            get { return this.content; }
            set { this.content = value; }
        }

        [XmlIgnore()]
        public bool IsAvailable
        {
            get { return (this.capacity > this.content.Count + this.reserved); }
        }

        [XmlIgnore()]
        public bool IsBlocked
        {
            get { return this.isBlocked; }
            set { this.isBlocked = value; }
        }

        [XmlIgnore()]
        public bool IsStarved       //IE486f18
        {
            get { return this.isStarved; }
            set { this.isStarved = value; }
        }

        [XmlIgnore()]
        public bool IsBusy
        {
            get { return (this.content.Count != 0); }
        }

        [XmlIgnore()]
        public bool IsIdle
        {
            get { return (this.content.Count == 0); }
        }

        [XmlIgnore()]
        public int Reserved
        {
            get { return this.reserved; }
            set { this.reserved = value; }
        }
        #endregion

        public void ChangeBlocked(double timeIn, bool valueIn)
        {
            this.IsBlocked = valueIn;
            Statistics blocked = this.Statistics["Blocked"];
            if (this.IsBlocked == true)
            {
                blocked.UpdateWeighted(timeIn, 1);
            }
            else
            {
                blocked.UpdateWeighted(timeIn, 0);
            }
        }

        public void ChangeStarved(double timeIn, bool valueIn)  //IE486f18
        {
            this.isStarved = valueIn;
            Statistics starved = this.Statistics["Starved"];
            if (this.isStarved == true)
            {
                starved.UpdateWeighted(timeIn, 1);
            }
            else
            {
                starved.UpdateWeighted(timeIn, 0);
            }
        }
    }

    [XmlType("StaticObjectState")]
    public class StaticObjectState : FLOWObjectState
    {
        private StringList content;

        private bool isBlocked;

        private bool isStarved;  //IE486f18

        private int reserved;

        public StaticObjectState()
        {
            this.content = new StringList();
        }

        [XmlArray("Content")]
        [XmlArrayItem(typeof(string))]
        public StringList Content
        {
            get { return this.content; }
            set { this.content = value; }
        }

        [XmlElement("IsBlocked")]
        public bool IsBlocked
        {
            get { return this.isBlocked; }
            set { this.isBlocked = value; }
        }

        [XmlElement("IsStarved")]       //IE486f18
        public bool IsStarved
        {
            get { return this.isStarved; }
            set { this.isStarved = value; }
        }

        [XmlElement("Reserved")]
        public int Reserved
        {
            get { return this.reserved; }
            set { this.reserved = value; }
        }

        public void GetState(StaticObject staticObjectIn)
        {
            base.GetState(staticObjectIn);
            foreach (MovableObject movableObject in staticObjectIn.Content)
            {
                this.content.Add(movableObject.Name);
            }
            this.isBlocked = staticObjectIn.IsBlocked;
            this.isStarved = staticObjectIn.IsStarved;  //IE486f18
            this.reserved = staticObjectIn.Reserved;
        }

        public void SetState(StaticObject staticObjectIn)
        {
            staticObjectIn.IsBlocked = this.isBlocked;
            staticObjectIn.IsStarved = this.isStarved;  //IE486f18
            staticObjectIn.Reserved = this.reserved;
            base.SetState(staticObjectIn);
        }
    }
}