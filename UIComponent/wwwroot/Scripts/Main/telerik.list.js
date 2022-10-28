(function ($) {
    var $t = $.telerik;
    var whiteSpaceRegExp = /\s+/;
    $t.list = {
        htmlBuilder: function (element, className, isSelect) {
            var val,
                text,
                id = element.id,
                name = element.name,
                builder = new $t.stringBuilder(),
                $element = $(element);


            function wrapper() {
                return $(['<div class="t-widget', className, 't-header"></div>'].join(" "));
            }

            this.render = function () {
                $element.wrap(wrapper()).hide();
                var $innerWrapper = $('<div class="t-dropdown-wrap t-state-default"></div>').insertBefore($element);
                this.text({
                    builder: builder,
                    text: text,
                    id: id,
                    name: name
                })
                .appendTo($innerWrapper);
                //button
                $('<span class="t-select"><span class="t-icon t-arrow-down">select</span></span>')
                .appendTo($innerWrapper);
                if (isSelect) {
                    builder.buffer = [];
                    $(builder
                        .cat('<input style="display:none;" type="text" ')
                        .catIf('value="', val, '" ', val)
                        .catIf('name="', name, '" ', name)
                        .cat('/>')
                        .string())
                    .insertAfter($innerWrapper);
                }
            }
            this.text = function (options) {
                return $(['<span class="t-input">', options.text || '&nbsp;', '</span>'].join(""));
            }
        },
        initialize: function () {
            $t.bind(this, {
                open: this.onOpen,
                close: this.onClose,
                valueChange: this.onChange
            });
        },
        common: function () {
            this.open = function () {
                if (this.data.length == 0) return;
                var $wrapper = this.$wrapper || this.$element, dropDown = this.dropDown;
                dropDown.element = this.element;
                var offset = $wrapper.position();
                offset.left = offset.left;
                var position = {
                    offset: offset,
                    outerHeight: $wrapper.outerHeight(),
                    outerWidth: $wrapper.outerWidth() + 2,
                    zIndex: $t.getElementZIndex($wrapper[0])
                };
                $t.hideErrorMessage($(this.element).closest('.t-dropdown')[0]);
                if (dropDown.$items) 
                    dropDown.open(position);
                else 
                    this.fill(function () { dropDown.open(position); });
                $(this.element).trigger("onOpen");
            }

            this.close = function () {
                $(this.element).trigger("onClose");
                this.dropDown.close();
            }

            this.focus = function () {
                debugger;
                var $txt = this.$wrapper.find('.t-input');
                this.$wrapper.focus();
                var value = $txt.val();
                if (value) {
                    var end = $txt.val().length;
                    $txt.setCursorPosition(0, end);
                }
            }
            this.dataBind = function (data) {
                this.data = data = (data || []);
                var index = -1;
                for (var i = 0, length = data.length; i < length; i++) {
                    var item = data[i];
                    if (item.text != null)
                        item.text = item.text.replace(/ي/gi, 'ی').replace(/ك/gi, "ک");
                    if (item) {
                        if (item.selected) {
                            index = i;
                        }
                        if (this.encoded) {
                            data[i].text = $t.encode(item.text);
                        }
                    }
                }
                this.dropDown.dataBind(data);

                if (index != -1) {
                    this.index = index;
                    this.select(index);
                }
            }

            this.highlight = function (argument) {
                var rebind = function (component) {
                    var previousValue = component.previousValue;
                    var dropDown = component.dropDown;
                    component.close();
                    dropDown.dataBind(component.data);
                    component.previousValue = previousValue;
                    dropDown.$items
                            .removeClass('t-state-selected')
                            .eq(index)
                            .addClass('t-state-selected');
                }
                var index = -1;

                if (!this.data) return index;

                if (!isNaN(argument - 0)) { // index
                    if (argument > -1 && argument < this.data.length) {

                        index = argument;

                        rebind(this);
                    }

                } else if ($.isFunction(argument)) { // predicate

                    for (var i = 0, len = this.data.length; i < len; i++) {
                        if (argument(this.data[i])) {
                            index = i;
                            break;
                        }
                    }
                    if (index != -1)
                        rebind(this);

                } else { // dom node

                    index = this.dropDown.highlight(argument);
                }
                return index;
            }
        },
        filtering: function () {
            this.filter = function (component) {
                component.isFiltered = true;

                var performAjax = true;
                var input = component.$text[0];
                var text = input.value.replace(/\ي/g, 'ی').replace(/\ك/g, "ک");
                var trigger = component.trigger;
                var dropDown = component.dropDown;
                text = this.multiple(text);

                if (text.length < component.minChars) return;

                var filterIndex = component.filter;
                performAjax = false;
                component.filters[filterIndex](component, component.data, text);

                if (!performAjax) {
                    var $items = dropDown.$items;
                    if (!$items) {
                        return;
                    }

                    var itemsLength = $items.length,
                        selectedIndex = component.selectedIndex;

                    var itemText = filterIndex == 0
                    ? selectedIndex != -1
                    ? $items[selectedIndex].innerText || $items[selectedIndex].textContent
                    : ''
                    : $items.length > 0
                    ? $items.first().text()
                    : '';

                    this.autoFill(component, itemText);

                    if (itemsLength == 0) {
                        trigger.close();
                    } else {
                        if (!dropDown.isOpened()) {
                            trigger.open();
                        }
                    }
                }
            }

            this.multiple = function (text) { return text; } // overriden by autocomplete
        },
        filters: function () { //mixin
            this.filters = [
                function firstMatch(component, data, inputText) {
                    if (!data || data.length == 0) return;
                    var dropDown = component.dropDown;
                    var $items = dropDown.$items;

                    if (!$items || $items.length == 0 || component.loader.isAjax()) {
                        dropDown.dataBind(data);
                        $items = dropDown.$items;
                    }

                    for (var i = 0, length = data.length; i < length; i++) {
                        if (data[i].text.slice(0, inputText.length).toLowerCase() == inputText.toLowerCase()) {
                            var item = $items[i];

                            component.selectedIndex = i;
                            dropDown.highlight(item);
                            dropDown.scrollTo(item);
                            return;
                        }
                    }

                    $items.removeClass('t-state-selected');
                    component.selectedIndex = -1;

                    $t.list.highlightFirstOnFilter(component, $items);
                },

                createItemsFilter(false, function (inputText, itemText) {
                    return itemText.slice(0, inputText.length).toLowerCase() == inputText.toLowerCase();
                }),

                createItemsFilter(true, function (inputText, itemText) {
                    if ((typeof itemText).toString() != "string")
                        return false;
                    return itemText && itemText.toLowerCase().indexOf(inputText.toLowerCase()) != -1
                })
            ]
        },
        loader: function (component) {
            this.ajaxError = false;
            this.component = component;
            this.isAjax = function () {
                return component.ajax || component.ws || component.onDataBinding;
            }
            function ajaxOptions(complete, options) {
                var result = {
                    url: (component.ajax || component.ws)['selectUrl'],
                    type: 'POST',
                    data: {},
                    dataType: 'text', // using 'text' instead of 'json' because of DateTime serialization
                    error: function (xhr, status) {
                        component.loader.ajaxError = true;
                        if ($t.ajaxError(component.element, 'error', xhr, status))
                            return;
                    },
                    complete: $.proxy(function () { this.hideBusy(); }, component.loader),
                    success: function (data, status, xhr) {
                        try {
                            data = eval('(' + data + ')');
                        } catch (e) {
                            // in case the result is not JSON raise the 'error' event
                            if (!$t.ajaxError(component.element, 'error', xhr, 'parseeror'))
                                throw new Error('Error! The requested URL did not return JSON.');
                            component.loader.ajaxError = true;
                            return;
                        }
                        data = data.d || data; // Support the `d` returned by MS Web Services 

                        if (complete)
                            complete.call(component, data);

                    }
                }
                $.extend(result, options);
                if (component.ws) {
                    result.data = $t.toJson(result.data);
                    result.contentType = 'application/json; charset=utf-8';
                }
                return result;
            }
            this.ajaxRequest = function (complete, options) {
                var e = {};
                if (options.data.text != "")
                    return;
                if ($t.trigger(component.element, 'dataBinding', e))
                    return;
                if (component.ajax || component.ws) {
                    this.showBusy();
                    $.ajax(ajaxOptions(complete, { data: $.extend({}, options ? options.data : {}, e.data) }));
                }
                else
                    if (complete) complete.call(component, component.data);
            },
            this.showBusy = function () {
                this.busyTimeout = setTimeout($.proxy(function () {
                    this.component.$wrapper.find('> .t-dropdown-wrap .t-icon').addClass('t-loading');
                }, this), 100);
            },
            this.hideBusy = function () {
                clearTimeout(this.busyTimeout);
                this.component.$wrapper.find('> .t-dropdown-wrap .t-icon').removeClass('t-loading');
            }
        },
        trigger: function (component) {
            this.component = component;

            this.open = function () {
                var dropDown = component.dropDown;
                if ((dropDown.$items && dropDown.$items.length > 0) && !dropDown.isOpened() && !$t.trigger(component.element, 'open'))
                    component.open();
            }

            this.close = function () {
                if (!component.dropDown.$element.is(':animated') && component.dropDown.isOpened() && !$t.trigger(component.element, 'close')) {
                    component.close();
                }
            };
        },
        retrieveData: function (select) {
            var items = [];
            var $options = $(select).find('option');

            for (var i = 0, length = $options.length; i < length; i++) {
                var $option = $options.eq(i);
                items[i] = {
                    text: $option.text(),
                    value: $option.val(),
                    Selected: $option.is(':selected')
                }
            }
            return items;
        },
        highlightFirstOnFilter: function (component, $items) {
            if (component.highlightFirst) {
                $items.first().addClass('t-state-selected');
                component.dropDown.scrollTo($items[0]);
            }
        },
        moveToEnd: function (element) {
            if (element.createTextRange) {
                var range = element.createTextRange();
                range.moveStart('textedit', 1);
                range.select();
            }
        },
        selection: function (input, start, end) {
            if (input.createTextRange) {
                var selRange = input.createTextRange();
                selRange.collapse(true);
                selRange.moveStart('character', start);
                selRange.moveEnd('character', end - start);
                selRange.select();
            } else if (input.selectionStart) {
                input.selectionStart = start;
                input.selectionEnd = end;
            }
        },
        getZIndex: function (element) {
            var zIndex = 'auto';
            $(element).parents().andSelf().each(function () { //get element 
                zIndex = $(this).css('zIndex');
                if (Number(zIndex)) {
                    zIndex = Number(zIndex) + 1;
                    return false;
                }
            });
            return zIndex;
        },
        keycodes: [8, // backspace
                   9, // tab
                  13, // enter
                  27, // escape
                  37, // left arrow
                  38, // up arrow
                  39, // right arrow
                  40, // down arrow
                  35, // end
                  36, // home
                  46] // delete
    }

    function createItemsFilter(global, condition) {
        return function (component, data, inputText) {
            if (!data || data.length == 0) return;

            var filteredDataIndexes = $.map(data, function (item, index) {
                if (condition(inputText, item.text || item)) return index;
            });

            var formatRegExp = new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + inputText.replace(/([\^\$\(\)\[\]\{\}\*\.\+\?\|\\])/gi, "\\$1") + ")(?![^<>]*>)(?![^&;]+;)", global ? 'ig' : 'i');

            component.filteredDataIndexes = filteredDataIndexes;
            component.selectedIndex = -1;

            component.dropDown.onItemCreate = function (e) { if (inputText) e.html = e.html.replace(formatRegExp, "<strong>$1</strong>"); }
            component.dropDown.dataBind($.map(filteredDataIndexes, function (item, index) {
                return data[item];
            }));

            var $items = component.dropDown.$items;
            $items.removeClass('t-state-selected');
            $t.list.highlightFirstOnFilter(component, $items);
        };
    }

    function firstMatch(data, $items, text) {
        if (!data || !$items) return null;

        var valueLength = text.length;
        text = text.toLowerCase();

        for (var i = 0, length = data.length; i < length; i++) {
            if (data[i].text.slice(0, valueLength).toLowerCase() == text)
                return $items[i];
        }
    }

    function enableLable(element) {
        var id = $(element).find('input').attr('id');
        if ($(element).hasClass('t-state-disabled'))
            $('label[for=' + id + ']').css('color', '#e5e2e2');
        else
            $('label[for=' + id + ']').css('color', '');
    }

    $t.dropDownList = function (element, options) {
        let _self = this;
        $.extend(this, options);
        let $div = $(element).closest('.t-dropdown');
        const resizeObserver = new ResizeObserver(() => {
            var pos = $div.position();
            $continner = $div.find('.t-animation-container'), $items = $continner.find('.t-group');
            $continner.width($div.width() - 4).css('left', pos.left + 2).css('top', pos.top + 34);
            $items.width($div.width() - 6);
        });
        resizeObserver.observe($div[0]);
        $(element).closest('.t-dropdown').focusin(function () {
            
            if (_self.enabled) {
                var $ctr = $(this).find('.t-dropdown-wrap');
                if ($ctr.hasClass('t-state-error'))
                    $(this).find('.t-dropdown-wrap').css('box-shadow', '0 0 5px 0 #f9183f');
                else
                    $(this).find('.t-dropdown-wrap').css('box-shadow', '0 0 3px 1px gray');
                if (_self.errorMessage)
                    $t.showErrorMessage(this, _self.errorMessage);
            }
        });
        $(element).closest('.t-dropdown').focusout(function () {
            $(this).find('.t-dropdown-wrap').css('box-shadow', 'none');
            $t.hideErrorMessage(this);
        });
        var isSelect = false;
        if (isSelect && !this.data) {
            this.data = $t.list.retrieveData(element);
            new $t.list.htmlBuilder(element, 't-dropdown', isSelect).render();
            element = element.previousSibling; //set element to input
        }
        var $dropdown = $(element).closest('.t-dropdown');
        $dropdown.mouseenter(function () {
            var $select = $(this).find('.t-dropdown-wrap');
            if ($select.hasClass('t-state-default'))
                $select.removeClass("t-state-default").addClass("t-state-hover");
        });
        $dropdown.mouseleave(function () {
            var $select = $(this).find('.t-dropdown-wrap');
            if ($select.hasClass('t-state-hover'))
                $select.removeClass("t-state-hover").addClass("t-state-default");
        });
        $dropdown.focusin(function () {
            $(element).trigger('onFocus')
            var $elemet = $(this).find('.t-dropdown-wrap');
            //if (!$elemet.hasClass('t-state-focused')) 
            //    $elemet.removeClass('t-state-hover').removeClass('t-state-default').addClass('t-state-focused');
        });
        $dropdown.focusout(function () {
            $(element).trigger('onBlure')
            var $element = $(this).find('.t-dropdown-wrap');
            //if ($element.hasClass('t-state-focused')) 
            //    $element.addClass('t-state-default').removeClass('t-state-focused');
        });
        var cachedInput = '';
        this.element = element;
        var $element = this.$element = $(element);

        this.loader = new $t.list.loader(this);
        this.trigger = new $t.list.trigger(this);
        this.$wrapper = $element.closest('.t-dropdown');
        this.$text = this.$wrapper.find('> .t-dropdown-wrap > .t-input');
        enableLable(this.$wrapper[0])
        //allow element to be focused
        if (!this.$wrapper.attr('tabIndex') && this.enabled) this.$wrapper.attr('tabIndex', 0);
        this.dropDown = new $t.dropDown({
            attr: this.dropDownAttr,
            effects: this.effects,
            onClick: $.proxy(function (e) {
                this.select(e.item);
                var index = $(e.item).closest('.t-reset').find('li').index(e.item);
                this.element.value = this.data[index].value;
                var event = new Event('change');
                this.element.dispatchEvent(event);
                //$(this.element).trigger('changeValue', value)
                this.trigger.close();
                this.$wrapper.focus();
            }, this)
        });

        //this.dropDown.$element.css('direction', this.$wrapper.closest('.t-rtl').length ? 'rtl' : '');
        this.fill = function (callback) {
            if (!this.dropDown.$items) {
                this.dataBind(this.data);
            }
        }

        this.updateState = function (options) {
            this.data = options.data;
            this.errorMessage = options.errorMessage;
            if (options.focused)
                this.$wrapper.focus();
            var value = $(this.element).val();
            this.enabled = !(options.disabled || false);
            if (this.enabled) {
                if (!this.$wrapper.attr('tabIndex'))
                    this.$wrapper.attr('tabIndex', 0);
            } else
                this.$wrapper.removeAttr('tabindex');
            var self_ = this;
            this.data.forEach(function (item, index) {
                if (item.value == value) {
                    if (!self_.dropDown.$items) 
                        self_.dataBind(self_.data);
                    self_.select(self_.dropDown.$items[index]);
                }
            });
        }
        this.enable = function () {
            this.$wrapper
                .removeClass('t-state-disabled')
                .bind({
                    keydown: $.proxy(keydown, this),
                    keypress: $.proxy(keypress, this),
                    click: $.proxy(function (e) {
                        var trigger = this.trigger;
                        var dropDown = this.dropDown;
                        this.$wrapper.focus();
                        if (dropDown.isOpened())
                            trigger.close();
                        else {
                            if (this.enabled) {
                                if (!dropDown.$items)
                                    this.fill(trigger.open);
                                trigger.open();
                            }
                        }
                    }, this)
                });
            enableLable(this.$wrapper[0])
        }

        this.select = function (item) {
            var index = this.highlight(item);
            if (index == -1) return index;
            if (this.selectedIndex && this.selectedIndex != index)
                $t.trigger(this.element, 'onChange');
            this.selectedIndex = index;
            //$t.list.updateTextAndValue(this, this.data[index].text, this.data[index].value);
        }

        $t.list.common.call(this);
        $t.list.initialize.call(this);

        $(document.documentElement).bind('mousedown', $.proxy(function (e) {
            //var $dropDown = this.dropDown.$element;
            //var isDropDown = $dropDown && $dropDown.parent().length > 0;
            //if (this.$wrapper[0] && $.contains(this.$wrapper[0], e.target)
            //    || (isDropDown && $.contains($dropDown.parent()[0], e.target)))
            //    return;
            ////this.trigger.change();
            //this.trigger.close();
        }, this));

        this[this.enabled ? 'enable' : 'disable']();

        // PRIVATE methods
        function resetTimer() {
            clearTimeout(this.timeout);
            this.timeout = setTimeout($.proxy(function () { cachedInput = '' }, this), 1000);
        }

        function keydown(e) {
            var trigger = this.trigger;
            var dropDown = this.dropDown;
            var key = e.keyCode || e.which;
            // close dropdown
            if (e.altKey && key == 38) {
                trigger.close();
                return;
            }

            // open dropdown
            if (e.altKey && key == 40) {
                trigger.open();
                return;
            }
            if (key > 34 && key < 41) {

                e.preventDefault();

                if (!dropDown.$items) {
                    this.fill();
                    return;
                }

                var $items = dropDown.$items,
                    $selectedItem = $($items[this.selectedIndex]);

                var $item = (key == 35) ? $items.last() // end
                         : (key == 36) ? $items.first() // home
                         : (key == 37 || key == 38) ? $selectedItem.prev() // up
                         : (key == 39 || key == 40) ? $selectedItem.next() // down
                         : [];

                if ($item.length) {
                    var item = $item[0];

                    this.select(item);
                    dropDown.scrollTo(item);

                    if (!dropDown.isOpened())
                        trigger.change();
                }
            }

            if (key == 8) {
                resetTimer();
                e.preventDefault();
                cachedInput = cachedInput.slice(0, -1);
            }

            if (key == 9 || key == 13 || key == 27) {
                //trigger.change();
                //trigger.close();
            }
        }

        function keypress(e) {
            var dropDown = this.dropDown;
            var key = e.keyCode || e.charCode;

            if (key == 0 || $.inArray(key, $t.list.keycodes) != -1 || e.ctrlKey || e.altKey || e.shiftKey) return;

            if (!dropDown.$items) {
                this.fill();
                return;
            }

            var tempInputValue = cachedInput;

            tempInputValue += String.fromCharCode(key);

            if (tempInputValue) {

                var item = firstMatch(this.data, dropDown.$items, tempInputValue);

                if (item) {
                    this.select(item);
                    dropDown.scrollTo(item);
                }

                cachedInput = tempInputValue;
            }

            resetTimer();
        }
    }

    $.fn.tDropDownList = function (options) {
        return $t.create(this, {
            name: 'tDropDownList',
            init: function (element, options) {
                return new $t.dropDownList(element, options)
            },
            options: options
        });
    };

    // default options
    $.fn.tDropDownList.defaults = {
        effects: $t.fx.slide.defaults(),
        accessible: false,
        index: 0,
        enabled: true,
        encoded: true
    };

})(jQuery);
