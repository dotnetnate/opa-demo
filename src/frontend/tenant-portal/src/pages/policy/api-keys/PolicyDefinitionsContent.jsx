import { useState, useEffect } from 'react';
import { DataGrid } from '@/components/data-grid/DataGrid';
import { DataGridRowSelectAll } from '@/components/data-grid/DataGridRowSelectAll';
import { DataGridRowSelect } from '@/components/data-grid/DataGridRowSelect';
import { PermissionEditor } from './PermissionEditor';

const PolicyDefinitionsContent = () => {
  const [data, setData] = useState([]);
  const [selectedRow, setSelectedRow] = useState(null);
  const [isEditorVisible, setEditorVisible] = useState(false);

  const columns = [
    {
      id: 'select',
      header: ({ table }) => <DataGridRowSelectAll />,
      cell: ({ row }) => <DataGridRowSelect row={row} />,
      size: 40,
    },
    {
      accessorKey: "name",
      header: "Permission Name",
    },
    {
      accessorKey: "appliesTo",
      header: "Applies To",
      cell: (row) => {
        const resourceTypes = row.getValue().map((item) => item.resourceTypes).join(", ");
        return resourceTypes;
      },
    },
    {
      accessorKey: "allowedConditions",
      header: "Has Conditions",
      cell: (row) => {
        return row?.getValue()?.length > 0 ? "Yes" : "No";
      },
    },
    {
      accessorKey: "visibility",
      header: "Enabled",
      cell: (item) => {
        return item?.getValue().enabled ? "Yes" : "No";
      },
    },
  ];

  const getData = async () => {
    try {
      const response = await fetch(
        'https://localhost:7040/api/tenant/dedf9bbb-a700-444b-b576-7bf93859f73d/settings/permissions'
      );
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      const doc = await response.json();
      return doc.permissionDefinitions;
    } catch (error) {
      console.error('Error fetching data:', error);
      return [];
    }
  };

  useEffect(() => {
    const fetchData = async () => {
      const result = await getData();
      setData(result);
    };

    fetchData();
  }, []);

  const handleRowDoubleClick = (row) => {
    console.log(row);
    setSelectedRow(row); // Pass the selected row to the editor
    setEditorVisible(true); // Show the modal
  };

  const handleSave = (updatedRow) => {
    setData((prevData) =>
      prevData.map((row) => (row.name === updatedRow.name ? updatedRow : row))
    );
    setEditorVisible(false); // Hide the modal
  };

  const handleClose = () => {
    setEditorVisible(false); // Hide the modal
  };

  return (
    <div className="container mx-auto py-10">
      <DataGrid
        columns={columns}
        data={data}
        rowSelection={true}
        onRowDoubleClick={handleRowDoubleClick}
      />
      {isEditorVisible && (


<PermissionEditor
  initialPermissionDefinition={selectedRow}
  onSave={(updatedDefinition) => {
    setData((prevData) =>
      prevData.map((row) =>
        row.name === updatedDefinition.name ? updatedDefinition : row
      )
    );
    setEditorVisible(false); // Hide the editor after saving
  }}
/>
        
        
      )}
    </div>
  );
};

export { PolicyDefinitionsContent };
