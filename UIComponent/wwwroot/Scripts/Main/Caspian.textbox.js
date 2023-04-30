(function ($) {
    let $t = $.caspian;
    function seprate3Digit(str, start, end, keyCode, seprate) {
        let array = str.replace('-', '').split('.');
        let str1 = array[0];
        let count = 0;
        if (seprate) {
            for (let i = 0; i < start && i < array[0].length; i++)
                if (str1.charAt(i) == ',')
                    count++;
            str1 = str1.replace(/\,/g, '');
            let strTemp = "", j = 1;
            for (let i = str1.length; i > 0; i--) {
                strTemp = str1.charAt(i - 1) + strTemp;
                if (j == 3) {
                    j = 0;
                    if (i > 1)
                        strTemp = ',' + strTemp
                }
                j++;
            }
            let newCount = 0;
            let tempStart = start;
            if (strTemp.length % 4 == 1 && keyCode != 8)
                tempStart = start + 1;
            if (strTemp.length % 4 == 3 && keyCode == 8)
                tempStart = start - 1;
            for (let i = 0; i < tempStart && i < strTemp.length; i++)
                if (strTemp.charAt(i) == ',')
                    newCount++;
        }
        else
            strTemp = array[0];
        if (array.length > 1)
            strTemp += '.' + array[1];
        let pos = start + 1;
        if (keyCode == 8)
            pos = start - 1;
        if (keyCode == 46)
            pos = start;
        if (seprate) {
            if (newCount < count && (keyCode == 8 || keyCode == 46))
                pos--;
            else
                if (newCount > count)
                    pos++;
        }
        if (str.length > 0 && str[0] == '-')
            strTemp = '-' + strTemp;
        return { text: strTemp, pos: pos };
    }
    function removeStr(str, start, end, keyCode, seprate) {
        if (start == end) {
            let strTemp = "";
            if (keyCode == 8) {
                for (let i = 0; i < str.length; i++)
                    if (i != start - 1 || str[start - 1] == ',')
                        strTemp += str.charAt(i);
            }
            else {
                for (let i = 0; i < str.length; i++)
                    if (i != start || str[start] == ',')
                        strTemp += str.charAt(i);
            }
        } else {
            let strTemp = "";
            for (let i = 0; i < str.length; i++)
                if (i < start || i >= end)
                    strTemp += str.charAt(i);
        }
       
        return seprate3Digit(strTemp, start, end, keyCode, seprate);
    }
    function updateValue(element, key, total, digit, seprate) {
        if (key >= 96 && key < 106)
            key -= 48;
        let value = $(element).val(), flag = key == 190 || key == 110;
        if (digit == 0 && key == 110 || key == 190)
            return;
        let selection = $(element).getSelection();
        if (key == 8 || key == 46) {
            let obj = removeStr(value, selection.start, selection.end, key, seprate);
            $(element).val(obj.text);
            $(element).setCursorPosition(obj.pos, obj.pos);
            return;
        }
        let index = value.indexOf('.');
        if (index > -1) {
            if (value.replace(/\,/g, '').length > total)
                return;
            flag = false;
            let array = value.split('.');
            if (selection.start > index && array[1].length >= digit)
                return;
        }
        else {
            if (!flag && value.replace(/\,/g, '').length >= total && key != 37 && key != 39) {
                return;
            }
        }
        if (selection.start == 0 && (key == 109 || key == 173) && (value.indexOf('-') == -1))
            flag = true;
        if (key >= 48 && key < 58 || flag) {
            let chr = key - 48;
            let len = value.length;
            if (key == 190 || key == 110) {
                chr = '.';
                if (value.length - selection.end > digit)
                    len = selection.end + digit;
            }
            if (key == 109 || key == 173) 
                chr = '-'
            let str = value.substr(0, selection.start) + chr + value.substring(selection.end, len);
            let obj = seprate3Digit(str, selection.start, selection.end, key, seprate);
            $(element).val(obj.text);
            $(element).setCursorPosition(obj.pos, obj.pos);
        }
        if (key == 35) {
            let len = $(element).val().length;
            $(element).setCursorPosition(len, len);
        }
        if (key == 36)
            $(element).setCursorPosition(0, 0);
        if (key == 37 && selection.start > 0)
            $(element).setCursorPosition(selection.start - 1, selection.start - 1);
        if (key == 39)
            $(element).setCursorPosition(selection.start + 1, selection.start + 1);

    }
    $t.textbox = function (element, options) {
        updateFlag = true;
        $.extend(this, options);
        $(element).mouseenter(function () {
            $(this).parent().addClass('t-state-hover');
        });
        $(element).mouseleave(function () {
            $(this).parent().removeClass('t-state-hover');
        });
        if (this.maskedText)
            $(element).mask(this.maskedText);
        this.id = $(element).attr('id');
        let thisObj = this;
        $(element).parent().find('a').click(function () {
            if (thisObj.enabled)
                thisObj.showHelpWindow();
        });
        this.element = element;
        let self = this;
        let $element = this.$element = $(element)
            .bind({
                focus: function (e) {
                    let input = e.target;
                    setTimeout(function () {
                        if ($.browser.msie)
                            input.select();
                        else {
                            input.selectionStart = 0;
                            input.selectionEnd = input.value.length;
                        }
                    }, 10);
                    $(input).parent().removeClass('t-state-hover');
                },
                keydown: $.proxy(this._keydown, this),
                keypress: $.proxy(this._keypress, this),
                keyup: function (e) {
                    thisObj.keyIsOperate = false;
                }
            }).bind("paste", $.proxy(this._paste, this));
        this.keyIsOperate = false;

        if (this.type != 'string') {
            this.numFormat = this.numFormat === undefined ? this.type.charAt(0) : this.numFormat;
            let separator = this.separator;
            this.step = this.parse(this.step, separator);
            this.val = this.parse(this.val, separator);
            this.minValue = this.parse(this.minValue, separator);
            this.maxValue = this.parse(this.maxValue, separator);
            this.decimals = { '190': '.', '188': ',', '110': separator };
        }
        $(element).blur(function (e) {
            
        });
        $(element).focus(function () {
            let message = $(element).closest('.t-widget').attr('data-bind');
            //if (message)
            //    $t.showErrorMessage($(self.element).closest('.t-widget')[0], message);
        });
        if (this.group && this.type != 'string') {
            $(element).val($t.get3Digit($(element).val()));
        }
    }

    $t.textbox.prototype = {
        selection: function () {
            return $(this.element).getSelection();
        },
        updateValue: function (value) {
            this.value(value);
            this._blur();
        },
        showErrorMessage: function (msg) {
            
        },
        setKeyValues: function (items) {
            let str = "";
            let array = new Array();
            for (let i = 0; i < items.length; i++) {
                let item = items[i];
                str += item.value;
                array.push(item.key);
                if (i < items.length - 1)
                    str += ' - ';
            }
            this.keyValue = array;
            this.$element.val(str);
        },
        _paste: function (e) {
            let val = this.$element.val();
            if ($.browser.msie) {
                let selectedText = this.element.document.selection.createRange().text;
                let text = window.clipboardData.getData("Text");
                if (selectedText && selectedText.length > 0) 
                    val = val.replace(selectedText, text);
                else 
                    val += text;
            }

            if (val == '-') return true;

            let parsedValue = this.parse(val, this.separator);
            if (parsedValue || parsedValue == 0) {
                this._update(parsedValue);
            }
        },
        updateState: function (options) {
            $.extend(this, options);
            if (options.focused) 
                this.focus();
            this.errorMessage = options.errorMessage;
        },
        _update: function (val) {
            if (this.val != val) {
                if ($t.trigger(this.element, 'valueChange', { oldValue: this.val, newValue: val })) {
                    val = this.val;
                }
            }
            this._value(val);
        },
        _value: function (value) {
            if (this.type != 'string') {
                let parsedValue = (typeof value === "number") ? value : this.parse(value, this.separator),
                text = this.enabled ? this.text : '',
                isNull = parsedValue === null;

                if (parsedValue != null) {
                    parsedValue = parseFloat(parsedValue.toFixed(this.digits));
                }
                if (this.group && value) {
                    this.$element.val($t.get3Digit(value));
                    this.$text.html($t.get3Digit(value));
                }
                else {
                    this.$element.val(isNull ? '' : this.formatEdit(parsedValue));
                    this.$text.html(isNull ? text : this.format(parsedValue));
                }
                this.val = parsedValue;
            }
            else {
                this.$element.val(this.parse(value));
                this.val = value;
                this.$text.html(value);
            }
        },
        value: function (value) {
            if (arguments.length == 1 && value == null) {
                this.$element.val('');
                return;
            }
            if (this.type == 'numeric') {
                if (arguments.length == 0) {
                    let val = this.$element.val().replace(/\,/g, '');
                    if (val == '')
                        return null;
                    if (this.maskedText && val.indexOf('_') != -1)
                        return null;
                    return parseFloat(val);
                }
                
                if (value && value.replace)
                    value = value.replace(/\,/g, '');
                let parsedValue = (typeof value === "number") ? value : this.parse(value, this.separator);
                if (!this.inRange(parsedValue, this.minValue, this.maxValue)) {
                    parsedValue = null;
                }

                this._value(parsedValue);
            }
            else {
                if (arguments.length == 0) {
                    let val = this.$element.val().replace(/\ي/g, 'ی').replace(/\ك/g, "ک");
                    if (this.maskedText && val.indexOf('_') != -1)
                        return null;
                    return val;
                }
                this._value(value);
            }
        },
        formatEdit: function (value) {
            let separator = this.separator;
            if (value && separator != '.')
                value = value.toString().replace('.', separator);
            return value;
        },
        format: function (value) {
            return $t.textbox.formatNumber(value,
                                           this.numFormat,
                                           this.digits,
                                           this.separator,
                                           this.groupSeparator,
                                           this.groupSize,
                                           this.positive,
                                           this.negative,
                                           this.symbol,
                                           true);
        },
        inRange: function (key, min, max) {
            return key === null || ((min !== null ? key >= min : true) && (max !== null ? key <= max : true));
        },
        parse: function (value, separator) {
            if (this.type != 'string') {
                let result = null;
                if (value || value == "0") {
                    if (typeof value == typeof 1)
                        return value;
                    value = value.replace(this.replaceRegExp, '');
                    if (separator && separator != '.')
                        value = value.replace(separator, '.');
                    let negativeFormatPattern = $.fn.tTextBox.patterns[this.type].negative[this.negative]
                        .replace(/(\(|\))/g, '\\$1').replace('*', '').replace('n', '([\\d|\\.]*)'),
                    negativeFormatRegEx = new RegExp(negativeFormatPattern);
                    if (negativeFormatRegEx.test(value))
                        result = -parseFloat(negativeFormatRegEx.exec(value)[1]);
                    else
                        result = parseFloat(value);
                }
                return isNaN(result) ? null : result;
            }
            else {
                return value;
            }
        }
    }
    $.fn.tTextBox = function (options) {
        let type = 'numeric';
        if (options && options.type)
            type = options.type;
        $.extend(this, options);
        options.type = type;
        return this.each(function () {
            let $element = $(this);
            options = $.meta ? $.extend({}, options, $element.data()) : options;

            if (!$element.data('tTextBox')) 
                $element.data('tTextBox', new $t.textbox(this, options));
        });
    };
})(jQuery);