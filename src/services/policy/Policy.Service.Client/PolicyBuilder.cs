using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policy.Service.Client {
    public class PolicyBuilder {
        private readonly Policy _policy;
        private readonly PolicyValidator _policyValidator = new();

        public PolicyBuilder(Resource resource) {
            if (string.IsNullOrEmpty(resource.Identifier) || string.IsNullOrEmpty(resource.Authority))
                throw new ArgumentException("ResourceId and ResourceType must be provided.");

            _policy = new Policy {
                Resource = resource,
                Rules = []
            };
        }

        public PolicyBuilder AddRule(Rule rule) {
            _policy.Rules.Add(rule);
            return this;
        }

        public PolicyBuilder AddOrUpdatePrivilege(Subject subject, Privilege privilege) {
            var rule = _policy.Rules.FirstOrDefault(r => r.Subject == subject);
            if (rule == null) {
                rule = new Rule {
                    Subject = subject
                };
                _policy.Rules.Add(rule);
            }
            else {
                var existingPrivilege = rule.Privileges.FirstOrDefault(p => string.Compare(p.PermissionName, privilege.PermissionName, StringComparison.InvariantCultureIgnoreCase) == 0);

                if (existingPrivilege != null) {
                    rule.Privileges.Remove(existingPrivilege);
                    rule.Privileges.Add(privilege);
                }
                else {
                    rule.Privileges.Add(privilege);
                }
            }

            rule.Privileges.Add(privilege);
            return this;
        }

        public PolicyBuilder RemoveRuleForSubject(Subject subject) {

            var subjectRule = _policy.Rules.FirstOrDefault(r => r.Subject.Equals( subject));

            if (subjectRule != null) {
                _policy.Rules.Remove(subjectRule);
            }
            return this;
        }
        public PolicyBuilder RemoveRuleForSubject(string subjectAuthority, string subjectId) {
            return RemoveRuleForSubject(new Subject { Authority = subjectAuthority, Identifier = subjectId });
        }

        public Policy Build() {
            var validationResult = _policyValidator.Validate(_policy);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            return _policy;
        }
    }
}
