
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;

#pragma warning disable 1591
namespace Smartlogic.Semaphore.Api.Schemas.Structure
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "SEMAPHORE",Namespace="",IsNullable = false)]
    
    public partial class Semaphore
    {
        private List<Parameter> pARAMETERSField;

        private Structure oM_STRUCTUREField;

        public Semaphore()
        {
            this.oM_STRUCTUREField = new Structure();
            this.pARAMETERSField = new List<Parameter>();
        }

        [XmlArray("PARAMETERS", Order = 0)]
        [XmlArrayItem("PARAMETER", IsNullable = false)]
        public List<Parameter> Parameters
        {
            get
            {
                return this.pARAMETERSField;
            }
            set
            {
                this.pARAMETERSField = value;
            }
        }

        [XmlElement("OM_STRUCTURE", Order = 1)]
        public Structure Structure
        {
            get
            {
                return this.oM_STRUCTUREField;
            }
            set
            {
                this.oM_STRUCTUREField = value;
            }
        }

    }

    [XmlType(AnonymousType = true)]
    
    public partial class Parameter
    {

        [XmlAttribute("NAME")]
        public string Name { get; set; }

        [XmlText()]
        public string Value { get; set; }

    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "OM_STRUCTURE", Namespace = "", IsNullable = false)]
    
    public partial class Structure
    {
        private List<Field> _facets;
        private List<Attribute> _attributes;
        private List<Note> _notes;
        private List<SettingDefinition> _metadata;
        private List<RelationDefinition> _equivalence;
        private List<RelationDefinition> _hierarchicalRelations;
        private List<RelationDefinition> _associativeRelations;
        private List<User> _users;
        private List<TermClass> _classes;

        public static Structure FromXmlString(string value)
        {
            var serializer = new XmlSerializer(typeof(Structure));
            using (var s = new StringReader(value))
            {
                if (s != null)
                {
                    var result = serializer.Deserialize(s) as Structure;
                    return result;
                }
            }

            return new Structure();
        }

        public string ToXmlString()
        {
            var Serializer = new XmlSerializer(typeof (Structure));

            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                Serializer.Serialize(memoryStream, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        [XmlArray("TERM_CLASSES", Order = 0)]
        [XmlArrayItem("TERM_CLASS", IsNullable = false)]
        public List<TermClass> Classes
        {
            get { return _classes; }
            set { _classes = value; }
        }

        public Structure()
        {
            this._users = new List<User>();
            this._associativeRelations = new List<RelationDefinition>();
            this._hierarchicalRelations = new List<RelationDefinition>();
            this._equivalence = new List<RelationDefinition>();
            this._metadata = new List<SettingDefinition>();
            this._notes = new List<Note>();
            this._attributes = new List<Attribute>();
            this._facets = new List<Field>();
            this._classes = new List<TermClass>();
        }

        [XmlArray("TERM_FACETS", Order = 1)]
        [XmlArrayItem("FIELD", IsNullable = false)]
        public List<Field> Facets
        {
            get
            {
                return this._facets;
            }
            set
            {
                this._facets = value;
            }
        }

        [XmlArray("TERM_ATTRIBUTES", Order = 2)]
        [XmlArrayItem("TERM_ATTRIBUTE", IsNullable = false)]
        public List<Attribute> Attributes
        {
            get
            {
                return this._attributes;
            }
            set
            {
                this._attributes = value;
            }
        }

        [XmlArray("TERM_NOTES", Order = 3)]
        [XmlArrayItem("TERM_NOTE", IsNullable = false)]
        public List<Note> Notes
        {
            get
            {
                return this._notes;
            }
            set
            {
                this._notes = value;
            }
        }

        [XmlArray("TERM_METADATA", Order = 4)]
        [XmlArrayItem("METADATA_DEF", IsNullable = false)]
        public List<SettingDefinition> Settings
        {
            get
            {
                return this._metadata;
            }
            set
            {
                this._metadata = value;
            }
        }

        [XmlArray("EQUIVALENCE_RELATIONS", Order = 5)]
        [XmlArrayItem("RELATION_DEF", IsNullable = false)]
        public List<RelationDefinition> EquivalenceRelations
        {
            get
            {
                return this._equivalence;
            }
            set
            {
                this._equivalence = value;
            }
        }

        [XmlArray("HIERARCHICAL_RELATIONS", Order = 6)]
        [XmlArrayItem("RELATION_DEF", IsNullable = false)]
        public List<RelationDefinition> HierarchicalRelations
        {
            get
            {
                return this._hierarchicalRelations;
            }
            set
            {
                this._hierarchicalRelations = value;
            }
        }

        [XmlArray("ASSOCIATIVE_RELATIONS", Order = 7)]
        [XmlArrayItem("RELATION_DEF", IsNullable = false)]
        public List<RelationDefinition> AssociativeRelations
        {
            get
            {
                return this._associativeRelations;
            }
            set
            {
                this._associativeRelations = value;
            }
        }

        [XmlArray("USERS", Order = 8)]
        [XmlArrayItem("USER_DEF", IsNullable = false)]
        public List<User> Users
        {
            get
            {
                return this._users;
            }
            set
            {
                this._users = value;
            }
        }
    }

    [DebuggerDisplay("ID:{ID}, Value:{Value}")]
    [XmlType(AnonymousType = true)]
    
    public partial class Field
    {
        [XmlAttribute()]
        public string ID { get; set; }

        [XmlText()]
        public string Value { get; set; }

    }

    [DebuggerDisplay("ID:{ID}, Value:{Value}")]
    [XmlType(AnonymousType = true)]
    
    public partial class TermClass
    {
        [XmlAttribute()]
        public string ID { get; set; }

        [XmlText()]
        public string Value { get; set; }

    }

    [XmlType(AnonymousType = true)]
    [DebuggerDisplay("ID:{ID}, Name:{Name}")]
    
    public partial class Attribute
    {
        [XmlAttribute()]
        public string ID { get; set; }

        [XmlAttribute("NAME")]
        public string Name { get; set; }

    }

    [XmlType(AnonymousType = true)]
    [DebuggerDisplay("ID:{ID}, Name:{Name}")]
    
    public partial class Note
    {

        [XmlAttribute()]
        public string ID { get; set; }

        [XmlAttribute("NAME")]
        public string Name { get; set; }

    }

    [XmlType(AnonymousType = true)]
    [DebuggerDisplay("ID:{ID}, Name:{Name}")]
    
    public partial class SettingDefinition
    {

        private List<SettingValue> vALUE_DEFField;

        [XmlAttribute()]
        public string ID { get; set; }

        [XmlAttribute("NAME")]
        public string Name { get; set; }


        public SettingDefinition()
        {
            this.vALUE_DEFField = new List<SettingValue>();
        }

        [XmlElement("VALUE_DEF", Order = 0)]
        public List<SettingValue> Values
        {
            get
            {
                return this.vALUE_DEFField;
            }
            set
            {
                this.vALUE_DEFField = value;
            }
        }
    }

    [XmlType(AnonymousType = true)]
    [DebuggerDisplay("ID:{ID}, Value:{Value}")]
    
    public partial class SettingValue
    {

        [XmlAttribute()]
        public string ID { get; set; }

        [XmlText()]
        public string Value { get; set; }

    }

    [XmlType(AnonymousType = true)]
    [DebuggerDisplay("ID:{ID}, Name:{Name}")]
    
    public partial class RelationDefinition
    {

        [XmlAttribute()]
        public string ID { get; set; }

        [XmlAttribute("NAME")]
        public string Name { get; set; }

        [XmlAttribute("REV_NAME")]
        public string ReciprocalName { get; set; }

        [XmlAttribute("ABRV")]
        public string Abbreviation { get; set; }

        [XmlAttribute("REV_ABRV")]
        public string ReciprocalAbbreviation { get; set; }

        [XmlAttribute("SYMMETRIC")]
        public bool IsSymmetric { get; set; }

    }


    [XmlType(AnonymousType = true)]
    
    public partial class User
    {
        [XmlAttribute()]
        public string ID { get; set; }

        [XmlAttribute("NAME")]
        public string Name { get; set; }

    }
}
#pragma warning restore 1591