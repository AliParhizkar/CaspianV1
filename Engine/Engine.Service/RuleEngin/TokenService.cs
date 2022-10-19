using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class TokenService : SimpleService<Token>
    {
        public TokenService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.constValue).Required(t => t.TokenType == TokenType.ConstValue);
            RuleFor(t => t.ConstValueType).Required(t => t.TokenType == TokenType.ConstValue);
            RuleFor(t => t.parameterType).Required(t => t.TokenType == TokenType.Oprand);
        }

        public async override Task<Token> AddAsync(Token item)
        {
            var list = await GetAll().Where(t => t.RuleId == item.RuleId).OrderBy(t => t.Id).ToListAsync(); ;
            list.Add(item);
            Parser parser = new Parser(list);
            parser.Parse();
            return await base.AddAsync(item);
        }

        public async Task<Token> RemoveAsync(int ruleId)
        {
            var rule = await new RuleService(ServiceScope).SingleAsync(ruleId);
            rule.IsValid = false;
            var item = await GetAll().Where(t => t.RuleId == ruleId).OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            if (item != null)
            {
                base.Remove(item);
                await SaveChangesAsync();
            }
            return item;
        }

        public void Clear(Token token)
        {
            //var q = kar.Tokens.Where(t => t.ItemId == token.ItemId);
            //kar.Tokens.Remove(q);
        }
    }
}
