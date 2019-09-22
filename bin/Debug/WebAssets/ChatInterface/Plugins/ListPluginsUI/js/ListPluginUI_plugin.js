prefix = "http://";
// host = "localhost";
// port = "20000";
api = "/MachineMessageApi";

function ListPluginsUI_SetHostPort(ihost, iport)
{
    host = ihost;
    port = iport;
}    

function ListPluginsUI_CallApi(content)
{
    url = prefix + host + ":" + port + api;

    $.ajax({
        contentType: 'application/json',
        data: JSON.stringify(content),
        type: 'POST',
        url: url,
        success: function (data, textStatus, jqXHR) {
            console.log("Success");
            console.log(data);
            // alert('Success');
        }
    });
}

function ListPluginsUI_RestartICAN()
{
    ListPluginsUI_CallApi("Start_ICAN.SIC");
}

function ListPluginsUI_Click_then_CheckItems_then_PostMessage(IndexChangedList)
{
    for (var i in IndexChangedList)
    {
        // alert('Hello');

        targetIndex = IndexChangedList[i];
        targetItem = ListPluginsUI_dataList[targetIndex];

        if (targetItem['enabled']) {
            ListPluginsUI_CallApi("[ICAN.SIC] EnablePlugin " + targetItem["name"]);
        }
        else {
            ListPluginsUI_CallApi("[ICAN.SIC] DisablePlugin " + targetItem["name"]);
        }
    }
}

function ListPluginsUI_Click_then_CheckItems()
{
    container = $('#ListPluginsUI_EnabledPluginsList tbody tr.item');

    var table = document.getElementById('ListPluginsUI_EnabledPluginsList');
    var trChilds = table.querySelector('tbody').childNodes;
    // console.log(trChilds);
    // console.log('Len ' + trChilds.length);

    configurationChanged = false;
    IndexChangedList = [];

    for (i = 0; i < trChilds.length; i++)
    {
        itemName = trChilds[i].textContent;
        itemValue = trChilds[i].childNodes[0].firstChild.checked;

        // console.log(itemName);
        // console.log(itemValue);
        // console.log([trChilds[i]]);

        if (itemName != ListPluginsUI_dataList[i]['name'] || trChilds[i].childNodes[0].firstChild.nodeName != "INPUT") {
            console.log('[ERROR] Logic error in "ListPluginsUI"');
            alert('[ERROR] Logic error in "ListPluginsUI"');
        }

        if (itemValue != ListPluginsUI_dataList[i]['enabled']) {
            configurationChanged = true;

            ListPluginsUI_dataList[i]['enabled'] = itemValue;

            IndexChangedList.push(i);
        }
    }

    ListPluginsUI_Click_then_CheckItems_then_PostMessage(IndexChangedList);
}

function ListPluginsUI_ClickHandler()
{
    setTimeout(ListPluginsUI_Click_then_CheckItems, 500);
}

function ListPluginsUI_AddRows(row)
{
    content = '';

    for (var i in row)
    {
        var isChecked = 'checked';
        if (!row[i].enabled) isChecked = '';
        content += '<tr onclick="ListPluginsUI_ClickHandler()"><td><input name="ListPluginsUI_checkbox" type="checkbox" ' + isChecked + '/></td><td class="mdl-data-table__cell--non-numeric">' + row[i].name + '</td></tr>';
    }

    $("#ListPluginsUI_EnabledPluginsList tbody").html(content);
}

function ListPluginsUI_Init()
{
    ListPluginsUI_AddRows(ListPluginsUI_dataList);
    
    $("#ListPluginsUI_Sync").click(function() {
        ListPluginsUI_CallApi("[ICAN.SIC] ListPlugins");
    });

    $("#ListPluginsUI_SoftRestart").click(function() {
        ListPluginsUI_CallApi('Start_ICAN.SIC');
    });
}

function ListPluginsUI_Init_delayed(dataList)
{
    ListPluginsUI_dataList = dataList;

    setTimeout(ListPluginsUI_Init, 500);
}

function ListPluginsUI_Destroy()
{
    $("#ListPluginsUI").html('~');
}

// $(document).ready(function() {
//     dataList = [{ name: 'Abc', enabled: true}, { name: 'Def', enabled: true}, { name: 'Ghi', enabled: true}];

//     ListPluginsUI_Init_delayed(dataList);
// });