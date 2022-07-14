(function ($) {
    var $w = $.workflow = {
        init: function (dotNetObjectReference, myData) {
            var tempdata = JSON.parse(myData);
            $w.dotNetObjectReference = dotNetObjectReference;
            if (window.goSamples)
                goSamples();
            var $ = go.GraphObject.make;
            myDiagram =
                $(go.Diagram, "myDiagramDiv",
                    {
                        initialContentAlignment: go.Spot.Center,
                        allowDrop: true,
                        "LinkDrawn": showLinkLabel,
                        "LinkRelinked": showLinkLabel,
                        "animationManager.duration": 800,
                        "undoManager.isEnabled": true
                    });
            myDiagram.addDiagramListener("Modified", function (e) {
                //if (myDiagram.isModified)
                //    if (idx < 0)
                //        document.title += "*";
                //    else
                //        if (idx >= 0)
                //            document.title = document.title.substr(0, idx);
            });
            function nodeStyle() {
                return [
                    new go.Binding("location", "loc", go.Point.parse).makeTwoWay(go.Point.stringify),
                    {
                        locationSpot: go.Spot.Center,
                        mouseEnter: function (e, obj) { $w.showPorts(obj.part, true); },
                        mouseLeave: function (e, obj) { $w.showPorts(obj.part, false); }
                    }
                ];
            }
            function makePort(name, spot, output, input) {
                return $(go.Shape, "Circle", {
                    fill: "transparent",
                    stroke: null,
                    desiredSize: new go.Size(8, 8),
                    alignment: spot,
                    alignmentFocus: spot,
                    portId: name,
                    fromSpot: spot,
                    toSpot: spot,
                    fromLinkable: output,
                    toLinkable: input,
                    cursor: "pointer"
                });
            }
            var lightText = 'whitesmoke';
            myDiagram.nodeTemplateMap.add("",
                $(go.Node, "Spot", nodeStyle(),
                    $(go.Panel, "Auto",
                        $(go.Shape, "RoundedRectangle",
                            {
                                fill: "#00A9C9",
                                stroke: null,
                                doubleClick: function (e, textBlock) {
                                    if (e.shift)
                                        $w.showFormForFieldsInit1(e);
                                    else
                                        if (e.control)
                                            $w.showFormForFieldsInit(e);
                                        else
                                            $w.setActivityProperty(textBlock);
                                }
                            },
                            new go.Binding("figure", "figure")),
                        $(go.TextBlock,
                            {
                                font: "bold 11pt Helvetica, Arial, sans-serif",
                                textAlign: "right",
                                stroke: lightText,
                                margin: 8,
                                maxSize: new go.Size(160, NaN),
                                wrap: go.TextBlock.WrapFit,
                                editable: false,
                                doubleClick: function (e, textBlock) {
                                    if (e.shift)
                                        $w.showFormForFieldsInit1(e);
                                    else
                                        if (e.control)
                                            $w.showFormForFieldsInit(e);
                                        else
                                            $w.setActivityProperty(textBlock);
                                }
                            },
                            new go.Binding("text").makeTwoWay())
                    ),
                    makePort("T", go.Spot.Top, false, true),
                    makePort("L", go.Spot.Left, true, true),
                    makePort("R", go.Spot.Right, true, true),
                    makePort("B", go.Spot.Bottom, true, false)
                ));

            myDiagram.nodeTemplateMap.add("Start",
                $(go.Node, "Spot", nodeStyle(),
                    $(go.Panel, "Auto",
                        $(go.Shape, "Circle",
                            { minSize: new go.Size(40, 40), fill: "#79C900", stroke: null }),
                        $(go.TextBlock, "Start", {
                            font: "bold 11pt Helvetica, Arial, sans-serif",
                            stroke: lightText
                        },
                            new go.Binding("text"))
                    ),
                    makePort("T", go.Spot.Top, true, false),
                    makePort("L", go.Spot.Left, true, false),
                    makePort("R", go.Spot.Right, true, false),
                    makePort("B", go.Spot.Bottom, true, false)
                ));

            myDiagram.nodeTemplateMap.add("Diamond",
                $(go.Node, "Spot", nodeStyle(),
                    $(go.Panel, "Auto",
                        $(go.Shape, "Diamond",
                            {
                                minSize: new go.Size(80, 50),
                                fill: "#00A9C9",
                                stroke: null,
                                doubleClick: function (e, obj) {
                                    $w.showFormForCheckMethods(e, obj);
                                }
                            }),
                        $(go.TextBlock, "End",
                            {
                                font: "bold 11pt Helvetica, Arial, sans-serif",
                                stroke: lightText,
                                doubleClick: function (e, obj) {
                                    $w.showFormForCheckMethods(e, obj);
                                }
                            },
                            new go.Binding("text"))
                    ),
                    makePort("T", go.Spot.Top, true, true),
                    makePort("L", go.Spot.Left, true, true),
                    makePort("R", go.Spot.Right, true, true),
                    makePort("B", go.Spot.Bottom, true, true)
                ));

            myDiagram.nodeTemplateMap.add("End",
                $(go.Node, "Spot", nodeStyle(),
                    $(go.Panel, "Auto",
                        $(go.Shape, "Circle",
                            { minSize: new go.Size(40, 40), fill: "#DC3C00", stroke: null }),
                        $(go.TextBlock, "End",
                            { font: "bold 11pt Helvetica, Arial, sans-serif", stroke: lightText },
                            new go.Binding("text"))
                    ),
                    makePort("T", go.Spot.Top, false, true),
                    makePort("L", go.Spot.Left, false, true),
                    makePort("R", go.Spot.Right, false, true),
                    makePort("B", go.Spot.Bottom, false, true)
                ));
            myDiagram.nodeTemplateMap.add("Parallelogram1",
                $(go.Node, "Spot", nodeStyle(),
                    $(go.Panel, "Auto",
                        $(go.Shape, "Parallelogram1",
                            {
                                minSize: new go.Size(40, 40),
                                fill: "#00A9C9",
                                stroke: null,
                                doubleClick: function (e) {
                                    showFormForProcessMethod(e);
                                }
                            }),
                        $(go.TextBlock, "End",
                            {
                                font: "bold 11pt Helvetica, Arial, sans-serif",
                                stroke: lightText,
                                doubleClick: function (e) {
                                    $w.showFormForProcessMethod(e);
                                }
                            },
                            new go.Binding("text"))
                    ),
                    makePort("T", go.Spot.Top, true, true),
                    makePort("L", go.Spot.Left, true, true),
                    makePort("R", go.Spot.Right, true, true),
                    makePort("B", go.Spot.Bottom, true, true)
                ));
            myDiagram.nodeTemplateMap.add("Waiter",
                $(go.Node, "Spot", nodeStyle(),
                    $(go.Panel, "Auto",
                        $(go.Shape, "AndGate",
                            {
                                minSize: new go.Size(40, 50),
                                fill: "#00A9C9",

                                stroke: null,
                                doubleClick: function (e) {
                                    $w.showFormForProcessMethod(e);
                                }
                            }),
                        $(go.TextBlock,
                            {
                                font: "bold 11pt Helvetica, Arial, sans-serif",
                                stroke: lightText,
                                //minSize: new go.Size(50, 80),
                                textAlign: "center",
                                doubleClick: function (e) {
                                    $w.showFormForProcessMethod(e);
                                }
                            },
                            new go.Binding("text"))
                    ),
                    makePort("T", go.Spot.Top, false, false),
                    makePort("L", go.Spot.Left, false, true),
                    makePort("R", go.Spot.Right, true, false),
                    makePort("B", go.Spot.Bottom, false, false)
                ));
            go.Shape.defineFigureGenerator("File", function (shape, w, h) {
                var geo = new go.Geometry();
                var fig = new go.PathFigure(0, 0, true); // starting point
                geo.add(fig);
                fig.add(new go.PathSegment(go.PathSegment.Line, .75 * w, 0));
                fig.add(new go.PathSegment(go.PathSegment.Line, w, .25 * h));
                fig.add(new go.PathSegment(go.PathSegment.Line, w, h));
                fig.add(new go.PathSegment(go.PathSegment.Line, 0, h).close());
                var fig2 = new go.PathFigure(.75 * w, 0, false);
                geo.add(fig2);
                // The Fold
                fig2.add(new go.PathSegment(go.PathSegment.Line, .75 * w, .25 * h));
                fig2.add(new go.PathSegment(go.PathSegment.Line, w, .25 * h));
                geo.spot1 = new go.Spot(0, .25);
                geo.spot2 = go.Spot.BottomRight;
                return geo;
            });
            myDiagram.nodeTemplateMap.add("Comment",
                $(go.Node, "Auto", nodeStyle(),
                    $(go.Shape, "File",//Parallelogram1
                        { fill: "#EFFAB4", stroke: null }),
                    $(go.TextBlock,
                        {
                            margin: 5,
                            maxSize: new go.Size(200, NaN),
                            wrap: go.TextBlock.WrapFit,
                            textAlign: "center",
                            editable: true,
                            font: "bold 12pt Helvetica, Arial, sans-serif",
                            stroke: '#454545'
                        },
                        new go.Binding("text").makeTwoWay())
                ));
            myDiagram.linkTemplate =
                $(go.Link,
                    {
                        routing: go.Link.Normal,
                        corner: 5, toShortLength: 4,
                        relinkableFrom: true,
                        relinkableTo: true,
                        reshapable: true,
                        resegmentable: true,
                        mouseEnter: function (e, link) {
                            //link.findObject("HIGHLIGHT").stroke = "rgba(30,144,255,0.2)";
                        },

                        mouseLeave: function (e, link) {
                            //link.findObject("HIGHLIGHT").stroke = "transparent";
                        },
                        doubleClick: function (e, link) {
                            var label = link.findObject("LABEL");
                            if (label !== null) {
                                var text = label.part.data.text;
                                label.visible = text != null;
                            }
                            $w.connectorText(link);
                        }
                    },
                    new go.Binding("points").makeTwoWay(),
                    $(go.Shape,
                        {
                            isPanelMain: true,
                            stroke: "gray",
                            strokeWidth: 2

                        }),
                    $(go.Shape,
                        {
                            toArrow: "Standard",
                            stroke: null,
                            fill: "gray"
                        }),
                    $(go.Panel, "Auto",
                        {
                            //visible: go.Binding("text") != undefined,
                            name: "LABEL",
                            segmentIndex: 1,
                            segmentFraction: 0.5,
                            segmentOffset: new go.Point(0, 0),
                            segmentOrientation: go.Link.OrientUpright
                        },
                        new go.Binding("visible", "visible").makeTwoWay(),
                        $(go.Shape, "RoundedRectangle",
                            {
                                fill: "#F8F8F8",
                                stroke: null
                            }),
                        $(go.TextBlock, "",
                            {
                                textAlign: "center",
                                font: "10pt b nazanin, arial, sans-serif",
                                stroke: "#333333",
                                margin: 2,
                                editable: false
                            },
                            new go.Binding("text").makeTwoWay())
                    )
                );
            function showLinkLabel(e) {
                var label = e.subject.findObject("LABEL");
                if (label !== null)
                    label.visible = true;
            }
            $w.load(myData);
            myPalette =
                $(go.Palette, "myPaletteDiv",
                    {
                        "animationManager.duration": 800,
                        nodeTemplateMap: myDiagram.nodeTemplateMap,
                        model: new go.GraphLinksModel(
                            [
                                {
                                    category: "Start",
                                    text: "شروع"
                                },
                                {
                                    text: "پردازش کاربر"
                                },
                                {
                                    text: "پردازش\nسیستمی",
                                    figure: "Diamond",
                                    category: "Diamond"
                                },
                                {
                                    category: "End",
                                    text: "پایان"
                                },
                                {
                                    category: "Comment",
                                    text: "توضیحات"
                                },
                                {
                                    category: "Parallelogram1",
                                    text: "پردازش سیستم"
                                },
                                {
                                    category: "Waiter",
                                    text: "تائید جمعی"
                                }
                            ])
                    });
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

        WorkflowForm: function (dotnet) {
            $.workflowForm = {
                dotnet: dotnet
            };
        },

        findCode: function (formName, ctrName, propertyName, code) {
            var data = {
                action: 'findCode',
                content: JSON.stringify({
                    Id: ctrName,
                    Property: propertyName,
                    Code: code,
                    ClassName: formName
                })
            };
            window.chrome.webview.postMessage(data);
        },

        findEventHandler: function (formName, ctrName, eventName) {
            var data = {
                action: 'findEventHandler',
                content: JSON.stringify({
                    Id: ctrName,
                    EventHandler: eventName,
                    ClassName: formName
                })
            };
            window.chrome.webview.postMessage(data);
        },

        getSourceCodeString() {
            $.workflowForm.dotnet.invokeMethodAsync("GetSourceCodeString").then(t => {
                var data = {
                    action: 'setSourceCode',
                    content: t
                };
                window.chrome.webview.postMessage(data);
            });
        },

        getCodebehindString() {
            $.workflowForm.dotnet.invokeMethodAsync("GetCodebehindString").then(t => {
                var data = {
                    action: 'setCodebehind',
                    content: t
                };
                window.chrome.webview.postMessage(data);
            });
        },
        sendSaveRequest: function () {
            var data = {
                action: 'sendSourceCode',
                content: ''
            };
            window.chrome.webview.postMessage(data);;
        },
        loadForm: function (formId) {
            var data = {
                action: 'sendSourceCode',
                content: formId
            };
            window.chrome.webview.postMessage(data);;
        },
        saveCodeFile: function (code) {
            $.workflowForm.dotnet.invokeMethodAsync('SaveFile', code);
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

    
    