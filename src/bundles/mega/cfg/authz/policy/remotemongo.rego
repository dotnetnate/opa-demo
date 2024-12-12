package cfg.authz.policy.remotemongo

import rego.v1

default allow := false



# Matches the resource and subject and ensures that the specified permission is set to 'grant'.
# If it is, then evaluates the conditions in the context.
allow if {

		resourceId := input.resource.id
		subjectId := input.subject.id
		permission := input.action.id

		policy_documents := mongodb.find({
			"collection": "policies",
			"database": "opa-demo",
			"uri": "mongodb://mongodb:27017",
			"auth": {
				"username": "root",
				"password": "example"
			},
			"filter":{
				"resource.resourceId": resourceId,
				"rules.subject.identifier": subjectId
			},
			 "options": {"projection": {"_id": false}}
		})

		some policyKey
		policy_documents.results[policyKey].resource.resourceId == resourceId
    	policy := policy_documents.results[policyKey]

        
        some ruleKey        
		policy.rules[ruleKey].subject.identifier = subjectId        
		policy.rules[ruleKey].privileges[permission].effect == "permit"
        rule := policy.rules[ruleKey]
		
        all_constraints_match(rule.privileges[permission].conditions)              		
    
}


# Function to check all constraints
all_constraints_match(constraints) if {
	every c in constraints {
		some key        
        value:=input.action.context[key]
        key == c.attribute                
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

