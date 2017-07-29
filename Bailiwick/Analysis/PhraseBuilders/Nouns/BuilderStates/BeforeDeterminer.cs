using Bailiwick.Models;

namespace Bailiwick.Analysis.PhraseBuilders.Nouns.BuilderStates
{
    internal class BeforeDeterminer : BaseState
    {
        public override WordClassType GeneralWordClass
        {
            get { return WordClassType.Determiner; }
        }

        public override IState OnAdposition(IContext c)
        {
            if (c.Current.PartOfSpeech.Specific == "IO")
                return Adposition;

            return base.OnAdposition(c);
        }

        public override IState OnArticle(IContext c)
        {
            if( c.Current.PartOfSpeech.Specific == "AT" || c.Current.PartOfSpeech.Specific == "APPGE" )
              return ChooseArticleState(c);

            return base.OnArticle(c);
        }

        public override IState OnDeterminer(IContext c)
        {
            switch (c.Current.Normalized)
            {
                case "this":
                case "that":
                case "these":
                case "those":
                    return Determiner;
            }

            c.PushAccepted();
            return Determiner;
        }

        public override IState OnPronoun(IContext c)
        {
            switch ( c.Current.PartOfSpeech.Specific )
            {
                case "PPGE":
                case "PPX1":
                case "PPX2":
                case "PPY":
                    return Pronoun;
            }

            c.PushAccepted();
            return Initial.OnPronoun(c);
        }
    }
}
