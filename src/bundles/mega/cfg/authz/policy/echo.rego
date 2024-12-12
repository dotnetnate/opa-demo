package cfg.authz.policy.echo

import rego.v1

default allow := false


# Matches the resource and subject and ensures that the specified permission is set to 'grant'.
# If it is, then evaluates the conditions in the context.
allow if {
		input.resource.id == "89a0a9f8-71b6-49ee-a874-f6294d0bd753"
		#some policyKey
    	#data.opa.demo.policies.result.policies[policyKey].resource.resourceId == input.resource.id
    	#policy := data.opa.demo.policies.result.policies[policyKey]            
}


