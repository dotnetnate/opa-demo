package cfg.authz.policy.remotemongo2

import rego.v1

default allow := false
default decision := "deny"
default advices := []
default obligations := []


# Matches the resource and subject and ensures that the specified permission is set to 'grant'.
# If it is, then evaluates the conditions in the context.
allow if {

		resourceId := input.resource.identifier
		resourceAuthority := input.resource.authority
		subjectId := input.subject.identifier
		subjectAuthority := input.subject.authority
		permission := input.action.id

		policy_documents := mongodb.find({
			"collection": "policies_current",
			"database": "opa-demo",
			"uri": "mongodb://mongodb:27017",
			"auth": {
				"username": "root",
				"password": "example"
			},
			"filter":{
				"resource.identifier": resourceId,
				"resource.authority": resourceAuthority,
				"rules.subject.identifier": subjectId,
				"rules.subject.authority": subjectAuthority
			}
			#,"options": {"projection": {"_id": false}}
		})

		some policyKey
		policy_documents.results[policyKey].resource.identifier == resourceId
    	policy := policy_documents.results[policyKey]

        
        some ruleKey        		
		policy.rules[ruleKey].subject.identifier = subjectId        
		policy.rules[ruleKey].privileges[permission].effect == "grant"
        rule := policy.rules[ruleKey]
		
        #all_constraints_match(rule.privileges[permission].conditions)              		
    
}

decision := "permit" if allow


# Function to check all constraints
all_constraints_match(constraints) if {
	every c in constraints {
		some key        
        value:=input.context[key]
        key == c.contextAttributePath		
        constraint_check(c, value)
	}
}

# Constraint check function
constraint_check(constraint, value) if {
	constraint.operator == "lte"
	value <= constraint.value
}

constraint_check(constraint, value) if {
	constraint.operator == "gte"
	value >= constraint.value
}

constraint_check(constraint, value) if {
	constraint.operator == "eq"
	value == constraint.value
}

constraint_check(constraint, value) if {
	constraint.operator == "gt"
	value > constraint.value
}

constraint_check(constraint, value) if {
	constraint.operator == "lt"
	value < constraint.value
}

constraint_check(constraint, value) if {
	constraint.operator == "ne"
	value != constraint.value
}



