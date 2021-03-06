﻿namespace Owin.Scim.Tests.Versioning
{
    using System.Collections.Generic;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_different_default_multi_valued_attributes : when_generating_a_User_etags<ScimUser>
    {
        Establish ctx = () =>
        {
            User = new ScimUser2 { Emails = _Emails };
        };

        Because of = () =>
        {
            _Emails.Add(new Email { Type = "work", Value = "babs.jensen@work.com" });

            User1ETag = User.CalculateVersion();

            _Emails.Add(new Email { Type = "home", Value = "babs.jensen@home.com" });

            User2ETag = User.CalculateVersion();
        };

        It should_be_different_values = () => User1ETag.ShouldNotEqual(User2ETag);

        private static readonly List<Email> _Emails = new List<Email>();
    }
}