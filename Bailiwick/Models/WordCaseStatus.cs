using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bailiwick.Models
{
    public class WordCaseStatus
    {
        #region Properties

        public bool? UpperCase
        {
            get
            {
                return allUpperCase ?? (allUpperCase = new bool?());
            }
            set
            {
                allUpperCase = value;
            }
        }
        bool? allUpperCase;

        public bool? LowerCase
        {
            get
            {
                return allLowerCase ?? (allLowerCase = new bool?());
            }
            set
            {
                allLowerCase = value;
            }
        }
        bool? allLowerCase;

        public bool? TitleCase
        {
            get
            {
                return titleCase ?? (titleCase = new bool?());
            }
            set
            {
                titleCase = value;
            }
        }
        bool? titleCase;
	
        #endregion

        #region Methods
        
        public void Check(string s)
        {
            // Reset
            UpperCase = null;
            LowerCase = null;
            TitleCase = null;

            // Check the casing
            char c;
            for (int i = 0; i < s.Length; i++)
            {
                c = s[i];
                if (Char.IsLetter(c))
                {
                    UpperCase = char.IsUpper(c) & (UpperCase ?? true);
                    LowerCase = char.IsLower(c) & (LowerCase ?? true);

                    if( TitleCase == null )
                        TitleCase = Char.IsUpper(c);
                    else
                        TitleCase &= Char.IsLower(c);
                }
            }
        }
        
        public string Apply(string s)
        {
            var sb = new StringBuilder();
            char c;

            if (UpperCase ?? false)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    c = s[i];
                    if (Char.IsLetter(c))
                    {
                        sb.Append(Char.ToUpper(c));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            else if (LowerCase ?? false)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    c = s[i];
                    if (Char.IsLetter(c))
                    {
                        sb.Append(Char.ToLower(c));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            else if (TitleCase ?? false)
            {
                if (s.Length >= 1)
                {
                    c = s[0];
                    if (Char.IsLetter(c))
                    {
                        sb.Append(Char.ToUpper(c));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

                for (int i = 1; i < s.Length; i++)
                {
                    c = s[i];
                    if (Char.IsLetter(c))
                    {
                        sb.Append(Char.ToLower(c));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            else 
            {
                sb.Append(s);
            }

            return sb.ToString();
        }

        #endregion

    }
}
