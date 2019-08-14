// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Dt.Core;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace Dt.Auth
{
    public static class Config
    {

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                // 固定一个作用域，API访问细分控制放在API服务中
                new ApiResource(Glb.ApiResourceName)
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "dtc",
                    ClientSecrets = { new Secret("dtc".Sha256()) },

                    // 用户名密码模式
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    // 设置AccessToken过期时间：15天
                    AccessTokenLifetime = 1296000,
                    // 客户端可访问的ApiResource
                    AllowedScopes = { Glb.ApiResourceName },

                    // 不使用RefreshToken
                    //// RefreshToken的最长生命周期，默认30天
                    //AbsoluteRefreshTokenLifetime = 2592000,
                    //// 刷新令牌时，将刷新RefreshToken的生命周期，RefreshToken的总生命周期不会超过AbsoluteRefreshTokenLifetime。
                    //RefreshTokenExpiration = TokenExpiration.Sliding,
                    //// 滑动刷新令牌的生命周期，默认15天，如果15天内没有使用RefreshToken，那么RefreshToken将失效
                    //// 即便是在15天内一直有使用RefreshToken，RefreshToken的总生命周期不会超过30天
                    //SlidingRefreshTokenLifetime = 1296000,
                    //// 如果要获取RefreshToken，必须把AllowOfflineAccess设置为true
                    //AllowOfflineAccess = true,
                },
                new Client
                {
                    ClientId = "dts",
                    ClientSecrets = { new Secret("dts".Sha256()) },

                    // 客户端凭据模式，用于服务与服务之间直接交互访问资源
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { Glb.ApiResourceName }
                },
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",

                    Claims = new []
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",

                    Claims = new []
                    {
                        new Claim("name", "Bob"),
                        new Claim("website", "https://bob.com")
                    }
                }
            };
        }
    }
}