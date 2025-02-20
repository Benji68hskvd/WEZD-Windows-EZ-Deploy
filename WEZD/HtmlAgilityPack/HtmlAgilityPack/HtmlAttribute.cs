using System.Diagnostics;

namespace WEZD.HtmlAgilityPack.HtmlAgilityPack
{
	[DebuggerDisplay("Name: {OriginalName}, Value: {Value}")]
	public class HtmlAttribute : IComparable
	{
		private int _line;

		internal int _lineposition;

		internal string _name;

		internal int _namelength;

		internal int _namestartindex;

		internal HtmlDocument _ownerdocument;

		internal HtmlNode _ownernode;

		private AttributeValueQuote _quoteType = AttributeValueQuote.DoubleQuote;

		internal int _streamposition;

		internal string _value;

		internal int _valuelength;

		internal int _valuestartindex;

		internal bool _isFromParse;

		internal bool _hasEqual;

		private bool? _localUseOriginalName;

		public int Line
		{
			get => _line;
            internal set => _line = value;
        }

		public int LinePosition => _lineposition;

		public int ValueStartIndex => _valuestartindex;

		public int ValueLength => _valuelength;

		public bool UseOriginalName
		{
			get
			{
				bool result = false;
				if (_localUseOriginalName.HasValue)
				{
					result = _localUseOriginalName.Value;
				}
				else if (OwnerDocument != null)
				{
					result = OwnerDocument.OptionDefaultUseOriginalName;
				}
				return result;
			}
			set => _localUseOriginalName = value;
        }

		public string Name
		{
			get
			{
				if (_name == null)
				{
					_name = _ownerdocument.Text.Substring(_namestartindex, _namelength);
				}
				if (!UseOriginalName)
				{
					return _name.ToLowerInvariant();
				}
				return _name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_name = value;
				if (_ownernode != null)
				{
					_ownernode.SetChanged();
				}
			}
		}

		public string OriginalName => _name;

		public HtmlDocument OwnerDocument => _ownerdocument;

		public HtmlNode OwnerNode => _ownernode;

		public AttributeValueQuote QuoteType
		{
			get => _quoteType;
            set => _quoteType = value;
        }

        public AttributeValueQuote InternalQuoteType { get; set; }

		public int StreamPosition => _streamposition;

		public string Value
		{
			get
			{
				if (_value == null && _ownerdocument.Text == null && _valuestartindex == 0 && _valuelength == 0)
				{
					return null;
				}
				if (_value == null)
				{
					_value = _ownerdocument.Text.Substring(_valuestartindex, _valuelength);
					if (!_ownerdocument.BackwardCompatibility)
					{
						_value = HtmlEntity.DeEntitize(_value);
					}
				}
				return _value;
			}
			set
			{
				_value = value;
				if (_ownernode != null)
				{
					_ownernode.SetChanged();
				}
			}
		}

		public string DeEntitizeValue => HtmlEntity.DeEntitize(Value);

        public string XmlName => HtmlDocument.GetXmlName(Name, isAttribute: true, OwnerDocument.OptionPreserveXmlNamespaces);

        public string XmlValue => Value;

		public string XPath => ((OwnerNode == null) ? "/" : (OwnerNode.XPath + "/")) + GetRelativeXpath();

        public HtmlAttribute(HtmlDocument ownerdocument)
		{
			_ownerdocument = ownerdocument;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is HtmlAttribute htmlAttribute))
			{
				throw new ArgumentException("obj");
			}
			return Name.CompareTo(htmlAttribute.Name);
		}

		public HtmlAttribute Clone()
		{
			return new HtmlAttribute(_ownerdocument)
			{
				Name = OriginalName,
				Value = Value,
				QuoteType = QuoteType,
				InternalQuoteType = InternalQuoteType,
				_isFromParse = _isFromParse,
				_hasEqual = _hasEqual
			};
		}

		public void Remove()
		{
			_ownernode.Attributes.Remove(this);
		}

		private string GetRelativeXpath()
		{
			if (OwnerNode == null)
			{
				return Name;
			}
			int num = 1;
			foreach (HtmlAttribute item in (IEnumerable<HtmlAttribute>)OwnerNode.Attributes)
			{
				if (!(item.Name != Name))
				{
					if (item == this)
					{
						break;
					}
					num++;
				}
			}
			return "@" + Name + "[" + num + "]";
		}
	}
}
