namespace Owin.Scim.Tests.Integration.Groups.Update.remove
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;
    using Model.Groups;

    using v2.Model;

    public class with_path_and_filter : when_updating_a_group
    {
        Establish context = () =>
        {
            ExistingUser = CreateUser(new ScimUser2 {UserName = Users.UserNameUtility.GenerateUserName()});
            var anotherUser = CreateUser(new ScimUser2 {UserName = Users.UserNameUtility.GenerateUserName()});
            GroupToUpdate = CreateGroup(
                new ScimGroup2
                {
                    DisplayName = "groupToUpdate",
                    ExternalId = "externalId",
                    Members = new []
                    {
                        new Member {Value = anotherUser.Id, Type = "user"}
                    }
                });

            PatchGroupId = GroupToUpdate.Id;

            var stringFormat =
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                        ""Operations"": [{
                            ""op"":""remove"",
                            ""path"": ""members[type eq \""user\""]""
                        }]
                    }";

            PatchContent = new StringContent(
                stringFormat.Replace("{0}", ExistingUser.Id),
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_update_version = () => UpdatedGroup.Meta.Version.ShouldNotEqual(GroupToUpdate.Meta.Version);

        It should_update_last_modified = () => UpdatedGroup.Meta.LastModified.ShouldBeGreaterThan(GroupToUpdate.Meta.LastModified);

        It should_clear_members = () => UpdatedGroup.Members.ShouldBeNull();

        It should_retain_display_name = () => UpdatedGroup.DisplayName.ShouldEqual(GroupToUpdate.DisplayName);

        private static ScimGroup GroupToUpdate;
        private static ScimUser ExistingUser;
    }
}