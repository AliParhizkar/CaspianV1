(function ($) {
    var myDiagram;
    var $w = $.workflow = {
        init: function (dotNetObjectReference, myData) {
            let activities = JSON.parse(myData);
            $w.dotnet = dotNetObjectReference;
            let $ = go.GraphObject.make;
            myDiagram = $(go.Diagram, "workflowDesigner", {
                //grid: $(go.Panel, "Grid",  // a simple 10x10 grid
                //    $(go.Shape, "LineH", { stroke: "lightgray", strokeWidth: 0.5 }),
                //    $(go.Shape, "LineV", { stroke: "lightgray", strokeWidth: 0.5 })
                //)
            });
            myDiagram.nodeTemplateMap.add("UserActivity",
                $(go.Node, "Auto", new go.Binding("location", "loc", go.Point.parse),
                    $(go.Shape, "RoundedRectangle", {
                    fill: $(go.Brush, "Linear", { 1: "lightblue", 0: "#eef" }),
                    stroke: "lightblue", strokeWidth: 2,
                    click: $w.ActivitySelect,
                    angle: 90
                }), $(go.Panel, "Table", { defaultAlignment: go.Spot.TopRight, margin: 0, click: $w.ActivitySelect }, $(go.RowColumnDefinition, { column: 1, width: 0 }), $(go.Picture, {
                    source: "_content/Engine.Web/Content/Workflow/Images/UserActivity.png", row: 0, column: 0,
                    width: 120, height: 40, margin: 0,
                    
                }), $(go.TextBlock, 
                    new go.Binding("text", "title"), {
                    row: 1, column: 0,
                    textAlign: "center",
                    alignment: go.Spot.TopCenter,
                    height: 30,
                    width: 100,
                    font: "14px bold Arial",
                    margin: 5
                }))));

            myDiagram.nodeTemplateMap.add("ValidatorActivity",
                $(go.Node, "Auto", new go.Binding("location", "loc", go.Point.parse),
                    $(go.Shape, "RoundedRectangle", {
                        fill: $(go.Brush, "Linear", { 1: "#FFB322", 0: "#FFFDCF" }),
                        stroke: "lightblue", strokeWidth: 2,
                        click: $w.ActivitySelect,
                        angle: 90
                    }), $(go.Panel, "Table", { defaultAlignment: go.Spot.TopRight, margin: 0, click: $w.ActivitySelect }, $(go.RowColumnDefinition, { column: 1, width: 0 }), $(go.Picture, {
                        source: "_content/Engine.Web/Content/Workflow/Images/ValidatorActivity.png", row: 0, column: 0,
                        width: 120, height: 40, margin: 0,

                    }), $(go.TextBlock,
                        new go.Binding("text", "title"), {
                        row: 1, column: 0,
                        textAlign: "center",
                        alignment: go.Spot.TopCenter,
                        height: 30,
                        width: 100,
                        font: "14px bold Arial",
                        margin: 5
                    }))));

            myDiagram.nodeTemplateMap.add("Start",
                $(go.Node, "Table", new go.Binding("location", "loc", go.Point.parse),
                    $(go.Panel, "Spot",
                        $(go.Shape, "Circle",
                            { desiredSize: new go.Size(70, 70), fill: "transparent", stroke: "#09d3ac", strokeWidth: 3.5 }),
                        $(go.TextBlock, "Start", {text: "Start"})
                    )));



            let data = []
            activities.forEach(function (activity) {
                let loc = activity.Left + ' ' + activity.Top;
                console.log(activity.Id)
                let category = '';
                switch (activity.ActivityType) {
                    case 1:
                        category = 'Start';
                        break;
                    case 2:
                        category = 'UserActivity';
                        break;
                    case 3:
                        category = 'ValidatorActivity';
                        break;
                    
                }
                data.push({
                    category: category,
                    key: activity.Id,
                    title: activity.Title,
                    loc: loc
                });
            });
            myDiagram.model = new go.GraphLinksModel(data, [
                { from: "1", to: "2" },
                { from: "2", to: "3" },
                { from: "3", to: "4" },
                { from: "3", to: "5" }
            ]);
        },

        ActivitySelect: function (node, e) {
            let data = e.part.data;
            if (myDiagram.selection.count == 1) {
                myDiagram.selection.each(t => {
                    let data = t.part.data;
                });
            }

        },

        getActivityCodebehindString: function () {
            if (myDiagram.selection.count == 1) {
                myDiagram.selection.each(t => {
                    var id = t.part.data.key;
                    $w.dotnet.invokeMethodAsync("GetActivityCodebehindString", id).then(t => {
                        var data = {
                            action: 'setActivityCodebehind',
                            content: t
                        };
                        window.chrome.webview.postMessage(data);
                    });
                });
            }
        },


        getActivitySourceCodeString: function () {
            if (myDiagram.selection.count == 1) {
                myDiagram.selection.each(t => {
                    var id = t.part.data.key;
                    $w.dotnet.invokeMethodAsync("getActivitySourceCodeString", id).then(t => {
                        var data = {
                            action: 'setActivitySourceCode',
                            content: t
                        };
                        window.chrome.webview.postMessage(data);
                    });
                });
            }
        },








        connectorText: function (link) {
            switch (link.fromNode.data.category) {
                case '':
                case undefined:
                    $w.userDefineAction(link);
                    break;
                case 'Diamond':
                    $w.systemAction(link);
                    break;
            }
        },

        updateSelectedNodeData: function (text, value) {
            var tempNode = null, node = $w.curentNode;
            myDiagram.model.nodeDataArray.forEach(function (item) {
                if (item.__gohashid == node.part.data.__gohashid)
                    tempNode = item;
            });
            myDiagram.model.setDataProperty(tempNode, "text", text);
            node.part.data.actorType = value;
        },

        setActivityProperty: function (node) {
            var data = node.part.data;
            var activity = { title: data.text, actorType: data.actorType };
            $w.dotNetObjectReference.invokeMethodAsync('ShowActivityProperty', activity);
            $w.curentNode = node;
        },

        systemAction: function (link) {
            var obj = link.data;
            if (!obj)
                obj = {};
            obj.fromActivity = link.fromNode.data;
            $w.dotNetObjectReference.invokeMethodAsync('ShowMethodReturnRefrence', obj);
            $w.curentLink = link;
            //var url = checkActionsUrl;
            //$.telerik.post(url, obj, function (result) {
            //    $.telerik.unblockUI();
            //    var win = $.telerik.getWindow();
            //    win.size(300, 200);
            //    win.saveAction = function (data) {
            //        var tempLink = null;
            //        myDiagram.model.linkDataArray.forEach(function (item) {
            //            if (item.__gohashid == link.data.__gohashid)
            //                tempLink = item;
            //        });
            //        tempLink.compareType = data.compareType;
            //        tempLink.value = data.value;

            //        myDiagram.model.setDataProperty(tempLink, "text", data.title);
            //    };
            //    win.content(result);
            //    win.title('عملیات سیستمی');
            //    win.center();
            //    win.open();
            //});
        },

        updateLinkData: function (data) {
            console.log(data);
            tempLink = null;
            myDiagram.model.linkDataArray.forEach(function (item) {
                if (item.__gohashid == $w.curentLink.data.__gohashid)
                    tempLink = item;
            });
            if (!tempLink.data)
                tempLink.data = new Object();
            tempLink.checkValidation = data.checkValidation;
            tempLink.fieldName = data.fieldName;
            tempLink.compareType = data.compareType;
            tempLink.value = data.value;
            $w.curentLink.findObject("LABEL").visible = true;
            myDiagram.model.setDataProperty(tempLink, "text", data.title);
        },

        userDefineAction: function (link) {
            var data = link.data;
            //win.selectedObject = win.getObject();
            data.title = link.data.text;
            data.activity = {};
            $w.curentLink = link;
            $w.dotNetObjectReference.invokeMethodAsync('ShowUserAndProperyRefrence', data);
            //win.saveData = function (data) {

            //};
        },

        showPorts: function (node, show) {
            var diagram = node.diagram;
            if (!diagram || diagram.isReadOnly || !diagram.allowLink)
                return;
            node.ports.each(function (port) {
                port.stroke = (show ? "white" : null);
            });
        },

        load: function (data) {
            myDiagram.model = go.Model.fromJson(data);
            var allLinks = myDiagram.links;
            while (allLinks.next()) {
                var link = allLinks.value;
                myDiagram.model.linkDataArray.forEach(function (item) {
                    if (item.__gohashid == link.data.__gohashid) {
                        var label = link.findObject("LABEL");
                        if (label !== null) {
                            var text = label.part.data.text;
                            label.visible = text != null;
                        }
                    }
                });
            }
            
            myDiagram.model.linkDataArray.forEach(function (link) {
                link.visible = false;
            });
        },

        updateProcessActivity: function (action) {
            $w.curentNode.part.data.action = action;
            var tempNode = null, node = $w.curentNode;
            myDiagram.model.nodeDataArray.forEach(function (item) {
                if (item.__gohashid == node.part.data.__gohashid)
                    tempNode = item;
            });
            myDiagram.model.setDataProperty(tempNode, "text", action.faTitle);
        },

        showFormForProcessMethod: function (e) {
            let data = e.targetObject.part.data.action || {};
            $w.dotNetObjectReference.invokeMethodAsync('ShowProcessAction', data);
            $w.curentNode = e.targetObject;
            //$.telerik.post(processMethodsUrl, obj, function (result) {
            //    $.telerik.unblockUI();
            //    var win = $.telerik.getWindow();
            //    win.saveData = function (value, text) {
            //        e.targetObject.part.data.action = eval('(' + value + ')');
            //        e.targetObject.part.data.text = text;
            //        myDiagram.model.setDataProperty(e.targetObject, "text", text);
            //    }
            //    win.size(220, 300);
            //    win.content(result);
            //    win.center();
            //    win.open();
            //});
        },

        updateProcessMethod: function (action) {
            $w.curentNode.part.data.action = action;
            var tempNode = null, node = $w.curentNode;
            myDiagram.model.nodeDataArray.forEach(function (item) {
                if (item.__gohashid == node.part.data.__gohashid)
                    tempNode = item;
            });
            myDiagram.model.setDataProperty(tempNode, "text", action.faTitle);
        },

        showFormForCheckMethods: function (e, obj) {
            let data = obj.part.data.action;
            if (!data)
                data = {};
            $w.dotNetObjectReference.invokeMethodAsync('CheckMethods', data);
            $w.curentNode = obj;
            //$.telerik.blockUI();
            //$.telerik.post(checkMethodsUrl, action, function (result) {
            //    $.telerik.unblockUI();
            //    var win = $.telerik.getWindow();
            //    win.saveData = function (value, text) {
            //        obj.part.data.action = eval('(' + value + ')');
            //        myDiagram.model.setDataProperty(e.targetObject, "text", text);
            //    };
            //    win.size(320, 300);
            //    win.content(result);
            //    win.title('');
            //    win.center();
            //    win.open();
            //});
        },

        showFormForFieldsInit: function (e) {
            var data = e.targetObject.part.data;
            if (data.dynamicFields == null)
                data.dynamicFields = [];
            data.id = data.key;
            //win.saveFields = function (fields) {
            //    data.dynamicFields = fields;
            //};
            $w.dotNetObjectReference.invokeMethodAsync('ActivityDynamicField', data.dynamicFields);
        },

        showFormForFieldsInit1: function (e) {
            var data = e.targetObject.part.data;
            if (data.fields == null)
                data.fields = [];
            data.id = data.key;
            $w.dotNetObjectReference.invokeMethodAsync('ActivityField', data);
            //var win = $.telerik.getWindow();
            //win.selectedObject = data;

            //data.id = data.key;
            //win.formContentUrl = '/Learning/WorkFlowEngine/WorkflowGenrator/ActivityField';
            //win.saveFields = function (fields) {
            //    data.fields = fields;
            //};
            //win.size(350, 100);
            //win.data = data;
            //win.open();
        },

        save: function () {
            if (confirm('آیا با ثبت موافقید؟')) {
                var obj = new Object();
                obj.nodeDataArray = myDiagram.model.nodeDataArray;
                obj.linkDataArray = myDiagram.model.linkDataArray;
                $w.dotNetObjectReference.invokeMethodAsync('Save', JSON.stringify(obj));
            }
        }
    }
})(jQuery);
function save() {
}
