(function ($) {
    let $t = $.telerik;
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
    function enableLable(element) {
        let id = $(element).attr('id');

        if ($(element).is('[disabled=disabled]'))
            $('label[for=' + id + ']').css('color', '#e5e2e2');
        else
            $('label[for=' + id + ']').css('color', '');
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
        enableLable(element);
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
        let builder = new $t.stringBuilder();
        this.keyIsOperate = false;
        this.enabled = !$element.is('[disabled]');
        builder.buffer = [];
        builder.cat('[ |').cat(this.groupSeparator).catIf('|' + this.symbol, this.symbol).cat(']');
        this.replaceRegExp = new RegExp(builder.string(), 'g');
        builder.buffer = [];
        this.$text = $(builder.string()).insertBefore($element)
            .click(function (e) {
                element.focus();
            });
        this[this.enabled ? 'enable' : 'disable']();
        if (this.type != 'string') {
            this.numFormat = this.numFormat === undefined ? this.type.charAt(0) : this.numFormat;
            let separator = this.separator;
            this.step = this.parse(this.step, separator);
            this.val = this.parse(this.val, separator);
            this.minValue = this.parse(this.minValue, separator);
            this.maxValue = this.parse(this.maxValue, separator);
            this.decimals = { '190': '.', '188': ',', '110': separator };
        }
        $t.bind(this, {
            load: this.onLoad,
            valueChange: this.onValueChange,
            ekeyPress: this.onKeyPress,
            change: this.onChange
        });
        $(element).blur(function (e) {
            //e.preventDefault();
            $t.hideErrorMessage($(element).closest('.t-widget')[0], self.errorMessage);
            //thisObj._blur();
        });
        $(element).focus(function () {
            let message = $(element).closest('.t-widget').attr('data-bind');
            if (message)
                $t.showErrorMessage($(self.element).closest('.t-widget')[0], message);

            thisObj._focus();
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
            if (options.disabled) 
                this.disable()
            else if (options.disabled == false)
                this.enable();
            if (options.focused) 
                this.focus();
            this.errorMessage = options.errorMessage;
        },
        _keydown: function (e) {
            this.key = e.keyCode;
            let key = e.keyCode || e.which;
            if (this.simpleSearchUrl)
                this.type = "string";
            if (this.type != "string") {
                this.digits = this.digits || 0;
                this.total = this.total || 8;
                this.seprate = ',';
                let _self = this;
                if (this.group && key != 9) {
                    setTimeout($.proxy(function () {
                        let selection = $(_self.element).getSelection();
                        let oldValue = $(_self.element).val().substring(0, selection.start);
                        let oldCount = (oldValue.match(/,/g) || []).length;
                        let value = $(_self.element).val();
                        if (value.length == 0 || value[value.length - 1] != '.')
                            $(_self.element).val($.telerik.get3Digit(_self.value()));
                        let newValue = $(_self.element).val().substring(0, selection.start);
                        let newCount = (newValue.match(/,/g) || []).length;

                        let count = newCount - oldCount;
                        $(_self.element).setCursorPosition(selection.start + count, selection.end + count);

                    }, this), 0);
                }
                if ([8, 9, 13, 27, 37, 39, 35, 36, 46].indexOf(key) != -1)
                    return true;
                if (e.ctrlKey && (key == 67 || 86))
                    return true;
                let selection = $(_self.element).getSelection();
                if (selection.end > selection.start)
                    return true;
                
                if (key >= 48 && key <= 58 || key >= 96 && key < 106) {
                    let value = $(this.element).val().replace(/\,/g, '');
                    let hasDot = value.includes(".");
                    if (hasDot) {
                        let digitCount = this.total - this.digits;
                        let array = value.split('.');
                        if (selection.start <= array[0].length) {
                            if (array[0].length >= digitCount)
                                return false;
                        } if (array[1].length >= this.digits)
                            return false;
                    } else if (value.length >= this.total)
                        return false;
                    return true;
                } else if (this.digits && (key == 110 || key == 190))
                    return true;
                return false;
            }
            setTimeout($.proxy(function () {
                
                let key = e.keyCode || e.which;
                if (key == 8) {
                    this.keyIsOperate = true;
                }
                let self_ = this;
                if (this.searchForm) {
                    //this.searchForm.open();
                }
                self_.valueOld = self_.value();  
            }, this));
        },
        _keypress: function (e) {
            //key = e.keyCode || e.which;
            //$t.trigger(this.element, 'ekeyPress', e);
            //if (this.type == 'string') {
            //    if (this.checkNumeric && (key < 48 || key > 57))
            //        e.preventDefault();
            //    setTimeout($.proxy(function () {

            //    }, this));
            //}
        },
        focus: function () {
            this._focus();
            this.$element.focus();
        },
        _focus: function () {
            this.tempValue = this.value();
            this.$element.css('color', this.$text.css("color"));
            this.$text.hide();
        },
        _blur: function () {
            //this.$element.removeClass('t-state-error');
            //if (this.type != 'string') {
            //    let min = this.minValue,
            //    max = this.maxValue,
            //    parsedValue = this.parse(this.$element.val());
            //    if (parsedValue) {
            //        if (min != null && parsedValue < min) {
            //            parsedValue = min;
            //        } else if (max != null && parsedValue > max) {
            //            parsedValue = max;
            //        }
            //        parsedValue = parseFloat(parsedValue.toFixed(this.digits));
            //    }
            //}
            //else
            //    parsedValue = this.parse(this.$element.val());
        },
        _clearTimer: function (e) {
            clearTimeout(this.timeout);
            clearInterval(this.timer);
            clearInterval(this.acceleration);
        },
        _stepper: function (e, stepMod) {
            if (e.which == 1) {

                let step = this.step;

                this._modify(stepMod * step);

                this.timeout = setTimeout($.proxy(function () {
                    this.timer = setInterval($.proxy(function () {
                        this._modify(stepMod * step);
                    }, this), 80);

                    this.acceleration = setInterval(function () { step += 1; }, 1000);
                }, this), 200);
            }
        },
        _modify: function (step) {
            if (this.type != 'string') {
                let value = this.parse(this.element.value),
                min = this.minValue,
                max = this.maxValue;

                value = value ? value + step : step;

                if (min !== null && value < min) {
                    value = min;
                } else if (max !== null && value > max) {
                    value = max;
                }
                this._update(parseFloat(value.toFixed(this.digits)));
            }
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
        enable: function () {
            this.enabled = true;
            this.$element.removeAttr("disabled");
            this.$element.closest('.t-widget').removeClass('t-state-disabled');
            enableLable(this.element);
        },
        disable: function () {
            this.enabled = false;
            this.$element.attr('disabled', 'disabled');
            this.$element.closest('.t-widget').addClass('t-state-disabled');
            if (!this.val && this.val != 0)
                this.$text.html('');
            else if (true == $.browser.msie)
                this.$text.hide();
            enableLable(this.element);
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
        if (type != "string") {
            let defaults = $.fn.tTextBox.defaults[type];
            defaults.digits = $t.cultureInfo[type + 'decimaldigits'];
            defaults.separator = $t.cultureInfo[type + 'decimalseparator'];
            defaults.groupSize = $t.cultureInfo[type + 'groupsize'];
            defaults.positive = $t.cultureInfo[type + 'positive'];
            defaults.negative = $t.cultureInfo[type + 'negative'];
            defaults.symbol = $t.cultureInfo[type + 'symbol'];

            options = $.extend({}, defaults, options);
        }
        else
            $.extend(this, options);
        options.type = type;
        return this.each(function () {
            let $element = $(this);
            options = $.meta ? $.extend({}, options, $element.data()) : options;

            if (!$element.data('tTextBox')) {
                $element.data('tTextBox', new $t.textbox(this, options));
                $t.trigger(this, 'load');
            }

        });
    };
    let commonDefaults = {
        val: null,
        text: '',
        inputAttributes: ''
    };
    $.fn.tTextBox.defaults = {
        numeric: $.extend(commonDefaults, {
            minValue: -100,
            maxValue: 100
        }),
        currency: $.extend(commonDefaults, {
            minValue: 0,
            maxValue: 1000
        }),
        percent: $.extend(commonDefaults, {
            minValue: 0,
            maxValue: 100
        })
    };

    // * - placeholder for the symbol
    // n - placeholder for the number
    $.fn.tTextBox.patterns = {
        numeric: {
            negative: ['(n)', '-n', '- n', 'n-', 'n -']
        },
        currency: {
            positive: ['*n', 'n*', '* n', 'n *'],
            negative: ['(*n)', '-*n', '*-n', '*n-', '(n*)', '-n*', 'n-*', 'n*-', '-n *', '-* n', 'n *-', '* n-', '* -n', 'n- *', '(* n)', '(n *)']
        },
        percent: {
            positive: ['n *', 'n*', '*n'],
            negative: ['-n *', '-n*', '-*n']
        }
    };

    if (!$t.cultureInfo.numericnegative)
        $.extend($t.cultureInfo, { //default en-US settings
            currencydecimaldigits: 2,
            currencydecimalseparator: '.',
            currencygroupseparator: ',',
            currencygroupsize: 3,
            currencynegative: 0,
            currencypositive: 0,
            currencysymbol: '$',
            numericdecimaldigits: 2,
            numericdecimalseparator: '.',
            numericgroupseparator: ',',
            numericgroupsize: 3,
            numericnegative: 1,
            percentdecimaldigits: 2,
            percentdecimalseparator: '.',
            percentgroupseparator: ',',
            percentgroupsize: 3,
            percentnegative: 0,
            percentpositive: 0,
            percentsymbol: '%'
        });

    let customFormatRegEx = /[0#?]/;

    function reverse(str) {
        return str.split('').reverse().join('');
    }

    function injectInFormat(val, format, appendExtras) {
        let i = 0, j = 0,
            fLength = format.length,
            vLength = val.length,
            builder = new $t.stringBuilder();

        while (i < fLength && j < vLength && format.substring(i).search(customFormatRegEx) >= 0) {

            if (format.charAt(i).match(customFormatRegEx))
                builder.cat(val.charAt(j++));
            else
                builder.cat(format.charAt(i));

            i++;
        }

        builder.catIf(val.substring(j), j < vLength && appendExtras)
               .catIf(format.substring(i), i < fLength);

        let result = reverse(builder.string()),
            zeroIndex;

        if (result.indexOf('#') > -1)
            zeroIndex = result.indexOf('0');

        if (zeroIndex > -1) {
            let first = result.slice(0, zeroIndex),
                second = result.slice(zeroIndex, result.length);
            result = first.replace(/#/g, '') + second.replace(/#/g, '0');
        } else {
            result = result.replace(/#/g, '');
        }

        if (result.indexOf(',') == 0)
            result = result.replace(/,/g, '');

        return appendExtras ? result : reverse(result);
    }

    $t.textbox.formatNumber = function (number,
                                        format,
                                        digits,
                                        separator,
                                        groupSeparator,
                                        groupSize,
                                        positive,
                                        negative,
                                        symbol,
                                        isTextBox) {

        if (!format) return number;

        let type, customFormat, negativeFormat, zeroFormat, sign = number < 0;

        format = format.split(':');
        format = format.length > 1 ? format[1].replace('}', '') : format[0];

        let isCustomFormat = format.search(customFormatRegEx) != -1;

        if (isCustomFormat) {
            format = format.split(';');
            customFormat = format[0];
            negativeFormat = format[1];
            zeroFormat = format[2];
            format = (sign && negativeFormat ? negativeFormat : customFormat).indexOf('%') != -1 ? 'p' : 'n';
        }

        switch (format.toLowerCase()) {
            case 'd':
                return Math.round(number).toString();
            case 'c':
                type = 'currency'; break;
            case 'n':
                type = 'numeric'; break;
            case 'p':
                type = 'percent';
                if (!isTextBox) number = Math.abs(number) * 100;
                break;
            default:
                return number.toString();
        }

        let zeroPad = function (str, count, left) {
            for (let l = str.length; l < count; l++)
                str = left ? ('0' + str) : (str + '0');

            return str;
        }

        let addGroupSeparator = function (number, groupSeperator, groupSize) {
            if (groupSeparator && groupSize != 0) {
                let regExp = new RegExp('(-?[0-9]+)([0-9]{' + groupSize + '})');
                while (regExp.test(number)) {
                    number = number.replace(regExp, '$1' + groupSeperator + '$2');
                }
            }
            return number;
        }

        let cultureInfo = cultureInfo || $t.cultureInfo,
            patterns = $.fn.tTextBox.patterns,
            undefined;

        //define Number Formating info.
        digits = digits || digits === 0 ? digits : cultureInfo[type + 'decimaldigits'];
        separator = separator !== undefined ? separator : cultureInfo[type + 'decimalseparator'];
        groupSeparator = groupSeparator !== undefined ? groupSeparator : cultureInfo[type + 'groupseparator'];
        groupSize = groupSize || groupSize == 0 ? groupSize : cultureInfo[type + 'groupsize'];
        negative = negative || negative === 0 ? negative : cultureInfo[type + 'negative'];
        positive = positive || positive === 0 ? positive : cultureInfo[type + 'positive'];
        symbol = symbol || cultureInfo[type + 'symbol'];

        let exponent, left, right;

        if (isCustomFormat) {
            let splits = (sign && negativeFormat ? negativeFormat : customFormat).split('.'),
                leftF = splits[0],
                rightF = splits.length > 1 ? splits[1] : '',
                lastIndexZero = $t.lastIndexOf(rightF, '0'),
                lastIndexSharp = $t.lastIndexOf(rightF, '#');
            digits = (lastIndexSharp > lastIndexZero ? lastIndexSharp : lastIndexZero) + 1;
        }

        let factor = Math.pow(10, digits);
        let rounded = (Math.round(number * factor) / factor);
        number = isFinite(rounded) ? rounded : number;

        let split = number.toString().split(/e/i);
        exponent = split.length > 1 ? parseInt(split[1]) : 0;
        split = split[0].split('.');

        left = split[0];
        left = sign ? left.replace('-', '') : left;

        right = split.length > 1 ? split[1] : '';

        if (exponent) {
            if (!sign) {
                right = zeroPad(right, exponent, false);
                left += right.slice(0, exponent);
                right = right.substr(exponent);
            } else {
                left = zeroPad(left, exponent + 1, true);
                right = left.slice(exponent, left.length) + right;
                left = left.slice(0, exponent);
            }
        }

        let rightLength = right.length;
        if (digits < 1 || (isCustomFormat && lastIndexZero == -1 && rightLength === 0))
            right = ''
        else
            right = rightLength > digits ? right.slice(0, digits) : zeroPad(right, digits, false);

        let result;
        if (isCustomFormat) {
            if (left == 0) left = '';

            left = injectInFormat(reverse(left), reverse(leftF), true);
            left = leftF.indexOf(',') != -1 ? addGroupSeparator(left, groupSeparator, groupSize) : left;

            right = right && rightF ? injectInFormat(right, rightF) : '';

            result = number === 0 && zeroFormat ? zeroFormat
                : (sign && !negativeFormat ? '-' : '') + left + (right.length > 0 ? separator + right : '');

        } else {

            left = addGroupSeparator(left, groupSeparator, groupSize)
            patterns = patterns[type];
            let pattern = sign ? patterns['negative'][negative]
                        : symbol ? patterns['positive'][positive]
                        : null;

            let numberString = left + (right.length > 0 ? separator + right : '');

            result = pattern ? pattern.replace('n', numberString).replace('*', symbol) : numberString;
        }
        return result;
    }

    $.extend($t.formatters, {
        number: $t.textbox.formatNumber
    });
})(jQuery);