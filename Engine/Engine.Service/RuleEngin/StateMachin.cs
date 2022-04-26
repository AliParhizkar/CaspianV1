using System;
using System.Linq;
using System.Collections.Generic;

namespace Caspian.Engine
{
    /// <summary>
    /// برای اعتیار سنجی فرمول مورد استفاده قرار می گیرد.
    /// </summary>
    public class StateMachin
    {
        /// <summary>
        /// Satets of State machine
        /// </summary>
        private State[] States = new State[]
        {
            new State(){Id = 1, Status = StateStatus.Start},
            new State(){Id = 2, Status = StateStatus.Final},
            new State(){Id = 3, Status = StateStatus.Middle},
            new State(){Id = 4, Status = StateStatus.Middle},
            new State(){Id = 5, Status = StateStatus.Middle},
            new State(){Id = 6, Status = StateStatus.Middle},
            new State(){Id = 7, Status = StateStatus.Middle},
            new State(){Id = 8, Status = StateStatus.Middle}
        };

        /// <summary>
        /// Relations between satets
        /// </summary>
        private Relation[] Relations = new Relation[]
        {
            //--------------------------------------------------------------------------------------
            new Relation(){FromStateId = 1, ToStateId = 1, TokenKind = TokenKind.OpenBracket},
            new Relation(){FromStateId = 1, ToStateId = 2, TokenKind = TokenKind.Parameter},
            new Relation(){FromStateId = 1, ToStateId = 3, TokenKind = TokenKind.If},
            //--------------------------------------------------------------------------------------
            new Relation(){FromStateId = 2, ToStateId = 1, TokenKind = TokenKind.Math},
            new Relation(){FromStateId = 2, ToStateId = 1, TokenKind = TokenKind.Colon},
            new Relation(){FromStateId = 2, ToStateId = 2, TokenKind = TokenKind.CloseBracket},
            //---------------------------------------------------------------------------------------
            new Relation(){FromStateId = 3, ToStateId = 3, TokenKind = TokenKind.OpenBracket},
            new Relation(){FromStateId = 3, ToStateId = 4, TokenKind = TokenKind.Parameter},
            //---------------------------------------------------------------------------------------
            new Relation(){FromStateId = 4, ToStateId = 3, TokenKind = TokenKind.Math},
            new Relation(){FromStateId = 4, ToStateId = 4, TokenKind = TokenKind.CloseBracket},
            new Relation(){FromStateId = 4, ToStateId = 5, TokenKind = TokenKind.Compareable},
            //--------------------------------------------------------------------------------------
            new Relation(){FromStateId = 5, ToStateId = 5, TokenKind = TokenKind.OpenBracket},
            new Relation(){FromStateId = 5, ToStateId = 6, TokenKind = TokenKind.Parameter},
            //---------------------------------------------------------------------------------------
            new Relation(){FromStateId = 6, ToStateId = 3, TokenKind = TokenKind.Logical},
            new Relation(){FromStateId = 6, ToStateId = 5, TokenKind = TokenKind.Math},
            new Relation(){FromStateId = 6, ToStateId = 6, TokenKind = TokenKind.CloseBracket},
            new Relation(){FromStateId = 6, ToStateId = 7, TokenKind = TokenKind.QuestionMark},
            //----------------------------------------------------------------------------------------
            new Relation(){FromStateId = 7, ToStateId = 3, TokenKind = TokenKind.If},
            new Relation(){FromStateId = 7, ToStateId = 7, TokenKind = TokenKind.OpenBracket},
            new Relation(){FromStateId = 7, ToStateId = 8, TokenKind = TokenKind.Parameter},
            //---------------------------------------------------------------------------------
            new Relation(){FromStateId = 8, ToStateId = 1, TokenKind = TokenKind.Colon},
            new Relation(){FromStateId = 8, ToStateId = 7, TokenKind = TokenKind.Math},
            new Relation(){FromStateId = 8, ToStateId = 8, TokenKind = TokenKind.CloseBracket}
        };

        /// <summary>
        /// مشخصات Stateجاری
        /// </summary>
        public State CurentState { get; private set; }

        public StateMachin()
        {
            ///State جاری برابر State شروع می شود
            CurentState = States.Single(t => t.Status == StateStatus.Start);
        }

        public State Move(TokenKind tokenKind)
        {
            if (CurentState == null)
            {

            }
            else
            {
                var count = Relations.Count(t => t.TokenKind == tokenKind && t.FromStateId == CurentState.Id);
                if (count > 1)
                {

                }
            }
            var relation = Relations.SingleOrDefault(t => t.TokenKind == tokenKind && t.FromStateId == CurentState.Id);
            if (relation == null)
                throw new Exception("وضعیت نامعتبر", null);
            CurentState = States.Single(t => t.Id == relation.ToStateId);
            return CurentState;
        }

        public IList<TokenKind> ValidTokenKinds()
        {
            return Relations.Where(t => t.FromStateId == CurentState.Id).Select(t => t.TokenKind).ToList();
        }

        public bool IsFinalState()
        {
            return CurentState.Status == StateStatus.Final;
        }
    }


}
