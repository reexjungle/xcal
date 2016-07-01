using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.extensions;
using Sprache;

namespace reexjungle.xcal.infrastructure.serialization
{
    /// <summary>
    /// Represents a reader that provides fast, noncached, forward-only access to iCalendar data.
    /// </summary>
    public abstract class CalendarReader : TextReader
    {

        //literal Constants
        protected const char HTAB = '\u0009';
        protected const char EMPTY = '\0';
        protected const string DQUOTE = @"""";
        protected const string COMMA = ",";
        protected const string COLON = ":";
        protected const string SEMICOLON = ";";
        protected const string EscapedDQUOTE = @"\""";
        protected const string EscapedCOMMA = @"\,";
        protected const string EscapedCOLON = @"\:";
        protected const string EscapedSEMICOLON = @"\;";

        protected CalendarNodeType nodeType;

        public CalendarNodeType NodeType => nodeType;


        public virtual bool IsStartComponent()
        {
            throw new NotImplementedException();            
        }

        public virtual bool IsStartComponent(string name)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsComponent(string name)
        {
            throw new NotImplementedException();            
        }

        public virtual bool IsProperty(string name)
        {
            throw new NotImplementedException();              
        }

        public virtual bool IsParameter(string name)
        {
            throw new NotImplementedException();                  
        }

        public virtual bool IsValue()
        {
            throw new NotImplementedException();
        }


        public virtual CalendarReader ReadStartComponent(string name)
        {
            throw new NotImplementedException();

        }

        public virtual CalendarReader ReadEndComponent(string name)
        {
            throw new NotImplementedException();

        }

        public virtual CalendarReader ReadComponent(string name)
        {
            throw new NotImplementedException();

        }

        public virtual CalendarReader ReadProperty(string name)
        {
            throw new NotImplementedException();
        }

        public virtual CalendarReader ReadParameter(string name)
        {
            throw new NotImplementedException();
        }

        public virtual string ReadValue()
        {
            throw new NotImplementedException();
        }

        public virtual CalendarReader ReadToFollowing(string name)
        {
              throw new NotImplementedException();
        }

        public virtual CalendarReader ReadToNextSibling(string name)
        {
            throw new NotImplementedException();
        }

        public virtual string ReadContentAsBoolean()
        {
            throw new NotImplementedException();
        }
        public virtual string ReadContentAsString()
        {
            throw new NotImplementedException();
        }

        public virtual float ReadContentAsFloat()
        {
            throw new NotImplementedException();
        }

        public virtual int ReadContentAsInteger()
        {
           throw new NotImplementedException(); 
        }

        public virtual DateTime ReadContentAsDateTime()
        {
            throw new NotImplementedException();
        }

        public virtual TimeSpan ReadContentAsTimeSpan()
        {
            throw new NotImplementedException();
        }

        public virtual object ReadContentAsObject()
        {
            throw new NotImplementedException();
        }

        public virtual TContent ReadContent<TContent>(Func<TContent> ctor)
        {
            throw new NotImplementedException();
        }


        public new bool Read()
        {
            return base.Read() != -1;
        }

        public abstract bool MoveToFirstValue();

        public abstract bool MoveToNextValue();

        public abstract bool MoveToParameter();

        public abstract bool MoveToProperty();

        public abstract bool MoveToComponent();

        public virtual CalendarNodeType MoveToContent()
        {
            do
            {
                switch (nodeType)
                {
                    case CalendarNodeType.VALUE:
                        MoveToParameter();
                        goto case CalendarNodeType.PARAMETER;
                    case CalendarNodeType.PARAMETER:
                        MoveToProperty();
                        goto case CalendarNodeType.PROPERTY;
                    case CalendarNodeType.PROPERTY:
                        MoveToComponent();
                        goto case CalendarNodeType.COMPONENT;
                    case CalendarNodeType.COMPONENT:
                        return nodeType;
                }

            } while (Read());

            return nodeType;
        }

    }
}
