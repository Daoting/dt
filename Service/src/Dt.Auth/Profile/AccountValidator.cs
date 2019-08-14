#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-20 创建
******************************************************************************/
#endregion

#region 引用命名
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Security.Claims;
using System.Threading.Tasks;
#endregion

namespace Dt.Auth
{
    /// <summary>
    /// 授权模式为ResourceOwnerPassword时的账号密码校验
    /// </summary>
    public class AccountValidator : IResourceOwnerPasswordValidator
    {
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext p_context)
        {
            // p_context.UserName为User.ID
            long id;
            if (string.IsNullOrEmpty(p_context.UserName)
                || string.IsNullOrEmpty(p_context.Password)
                || !long.TryParse(p_context.UserName, out id))
            {
                p_context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "帐号或密码不可为空！");
                return;
            }

            User user = await Users.GetUserByID(id);
            p_context.Result = (user != null && user.Pwd == p_context.Password) ?
                    new GrantValidationResult(p_context.UserName, "custom", new Claim[] { new Claim("uid", p_context.UserName) })
                    : new GrantValidationResult(TokenRequestErrors.InvalidGrant, "帐号或密码错误！");
        }
    }
}
