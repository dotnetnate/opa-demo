using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NOCO.Threvw.Policy.Service.Client.Tests {
    [TestClass]
    public class PolicyBuilderTests {
        [TestMethod]
        public void Given_Valid_Resource_When_PolicyBuilder_Is_Constructed_Then_Policy_Is_Initialized() {
            var resource = new Resource { Identifier = "123", Authority = "Authority" };
            var policyBuilder = new PolicyBuilder(resource);

            Assert.IsNotNull(policyBuilder.Build());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Given_Invalid_Resource_When_PolicyBuilder_Is_Constructed_Then_Exception_Is_Thrown() {
            new PolicyBuilder(new Resource { Identifier = null, Authority = null });
        }

        [TestMethod]
        public void Given_Rule_When_AddRule_Is_Called_Then_Rule_Is_Added_To_Policy() {
            var resource = new Resource { Identifier = "123", Authority = "Authority" };
            var rule = new Rule { Subject = new Subject { Identifier = "subject1", Authority = "authority1" } };
            var policyBuilder = new PolicyBuilder(resource);

            policyBuilder.AddRule(rule);

            Assert.IsTrue(policyBuilder.Build().Rules.Contains(rule));
        }

        [TestMethod]
        public void Given_Subject_And_Privilege_When_AddOrUpdatePrivilege_Is_Called_Then_Privilege_Is_Added_To_Rule() {
            var resource = new Resource { Identifier = "123", Authority = "Authority" };
            var subject = new Subject { Identifier = "subject1", Authority = "authority1" };
            var privilege = new Privilege { PermissionName = "permission1" };
            var policyBuilder = new PolicyBuilder(resource);

            policyBuilder.AddOrUpdatePrivilege(subject, privilege);

            Assert.IsTrue(policyBuilder.Build().Rules.Any(r => r.Subject == subject && r.Privileges.Contains(privilege)));
        }

        [TestMethod]
        public void Given_Subject_And_Existing_Privilege_When_AddOrUpdatePrivilege_Is_Called_Then_Privilege_Is_Updated() {
            var resource = new Resource { Identifier = "123", Authority = "Authority" };
            var subject = new Subject { Identifier = "subject1", Authority = "authority1" };
            var privilege = new Privilege { PermissionName = "permission1" };
            var updatedPrivilege = new Privilege { PermissionName = "permission1", Action = PermissionActions.Grant};
            var policyBuilder = new PolicyBuilder(resource);

            policyBuilder.AddOrUpdatePrivilege(subject, privilege);
            policyBuilder.AddOrUpdatePrivilege(subject, updatedPrivilege);

            Assert.IsTrue(policyBuilder.Build().Rules.Any(r => r.Subject == subject && r.Privileges.Contains(updatedPrivilege)));
        }

        [TestMethod]
        public void Given_Subject_When_RemoveRuleForSubject_Is_Called_Then_Rule_Is_Removed_From_Policy() {
            var resource = new Resource { Identifier = "123", Authority = "Authority" };
            var subject = new Subject { Identifier = "subject1", Authority = "authority1" };
            var rule = new Rule { Subject = subject };
            var policyBuilder = new PolicyBuilder(resource);

            policyBuilder.AddRule(rule);
            policyBuilder.RemoveRuleForSubject(subject);

            Assert.IsFalse(policyBuilder.Build().Rules.Any(r => r.Subject == subject));
        }

        [TestMethod]
        public void Given_SubjectAuthority_And_SubjectId_When_RemoveRuleForSubject_Is_Called_Then_Rule_Is_Removed_From_Policy() {
            var resource = new Resource { Identifier = "123", Authority = "Authority" };
            var subject = new Subject { Identifier = "subject1", Authority = "authority1" };
            var rule = new Rule { Subject = subject };
            var policyBuilder = new PolicyBuilder(resource);

            policyBuilder.AddRule(rule);
            policyBuilder.RemoveRuleForSubject("authority1", "subject1");

            Assert.IsFalse(policyBuilder.Build().Rules.Any(r => r.Subject == subject));
        }

        [TestMethod]
        public void Given_Valid_Policy_When_Build_Is_Called_Then_Validated_Policy_Is_Returned() {
            var resource = new Resource { Identifier = "123", Authority = "Authority" };
            var policyBuilder = new PolicyBuilder(resource);

            var policy = policyBuilder.Build();

            Assert.IsNotNull(policy);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Given_Invalid_Policy_When_Build_Is_Called_Then_Exception_Is_Thrown() {
            var resource = new Resource { Identifier = "123", Authority = "Authority" };
            
            var policyBuilder = new PolicyBuilder(resource);

            policyBuilder.AddRule(new Rule { Subject = new Subject { Identifier = null, Authority = null} });

            var x = policyBuilder.Build();

            Assert.IsTrue(true);
        }
    }
}
