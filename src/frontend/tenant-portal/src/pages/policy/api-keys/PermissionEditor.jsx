import React, { useState } from 'react';
import { Accordion } from '@/components/accordion/Accordion';
import { AccordionItem } from '@/components/accordion/AccordionItem';
import { Checkbox } from '@/components/ui/checkbox';
import { Select, SelectGroup, SelectValue, SelectTrigger, SelectContent, SelectLabel, SelectItem, SelectSeparator, SelectScrollUpButton, SelectScrollDownButton } from '@/components/ui/select';

const PermissionEditor = ({ initialPermissionDefinition, onSave }) => {
  const [permissionDefinition, setPermissionDefinition] = useState(initialPermissionDefinition);

  if (!permissionDefinition?.allowedConditions || permissionDefinition.allowedConditions.length === 0) {
    return null; // Return null if there are no allowed conditions
  }

  const handleConditionChange = (index, field, newValue) => {
    const updatedConditions = permissionDefinition.allowedConditions.map((condition, i) =>
      i === index ? { ...condition, [field]: newValue } : condition
    );
    setPermissionDefinition({ ...permissionDefinition, allowedConditions: updatedConditions });
  };

  const handleSave = () => {
    if (onSave) {
      onSave(permissionDefinition); // Pass the updated permission definition back to the parent
    }
  };

  return (
    <div>
      <Accordion>
        {permissionDefinition.allowedConditions.map((condition, index) => (
          <AccordionItem key={index} title={`Attribute: ${condition.name}`}>
            <div className="flex items-center flex-wrap lg:flex-nowrap gap-2.5">
              <span className="form-label max-w-32 w-full">Attribute Name</span>
              <div className="grow min-w-48">
                <input
                  className="input w-full"
                  type="text"
                  value={condition.name}
                  onChange={(e) => handleConditionChange(index, 'name', e.target.value)}
                />
              </div>
            </div>
            <div className="flex items-center flex-wrap lg:flex-nowrap gap-2.5">
              <span className="form-label max-w-32 w-full">Data Type</span>
              <div className="grow min-w-48">                   
                <Select value={condition.dataType} onValueChange={value => {}} >
                <SelectTrigger className="grow min-w-48" size="default">
                    <SelectValue placeholder={condition.dataType} />
                  </SelectTrigger>
                  <SelectContent side="top">
                    <SelectItem key="number" value="number">Number</SelectItem>
                    <SelectItem key="string" value="string">String</SelectItem>
                    <SelectItem key="bool" value="bool">Bool</SelectItem>
                    <SelectItem key="date" value="date" >Date Time</SelectItem>
                    <SelectItem key="uuid" value="uuid" >UUID</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
            <div className="flex items-center flex-wrap lg:flex-nowrap gap-2.5">
              <span className="form-label max-w-32 w-full">Operator</span>
              <div className="grow min-w-48">
                <Checkbox checked={condition.required} onCheckedChange={(e) => handleConditionChange(index, 'required', e.target.value)} aria-label="Select row" className="align-[inherit]" />
              </div>
            </div>
          </AccordionItem>
        ))}
      </Accordion>
      <div className="flex justify-end mt-4">
        <button className="btn btn-primary" onClick={handleSave}>
          Save Changes
        </button>
      </div>
    </div>
  );
};

export { PermissionEditor };
