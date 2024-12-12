package cfg.authz.policy.richmongo

import rego.v1

default allow := false


# Matches the resource and subject and ensures that the specified permission is set to 'grant'.
# If it is, then evaluates the conditions in the context.
allow if {

		some policyKey
    	data.policydata.policies[policyKey].resource.resourceId == input.resource.id
    	policy := data.policydata.policies[policyKey]
        
        some ruleKey
        policy.rules[ruleKey].subject.identifier = input.subject.id
        policy.rules[ruleKey].privileges[input.action.id].effect == "permit"
        rule := policy.rules[ruleKey]
        
        all_constraints_match(rule.privileges[input.action.id].conditions)              
    
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

