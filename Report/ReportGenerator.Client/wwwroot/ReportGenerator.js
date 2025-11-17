/// <reference path="../../../uicomponent/wwwroot/typescripts/caspian.common.ts" />
var ReportGenerator;
(function (ReportGenerator) {
    var BackgroundImage = /** @class */ (function () {
        function BackgroundImage() {
        }
        BackgroundImage.Initialize = function () {
            var canvase = document.createElement('canvas');
            //canvase.style.width = '10cm';
            //canvase.style.height = '10cm';
            canvase.width = caspian.common.getPixelsPerCentimetre() * 50;
            canvase.height = caspian.common.getPixelsPerCentimetre() * 50;
            document.body.appendChild(canvase);
            BackgroundImage.context = canvase.getContext('2d');
            BackgroundImage.createImage();
            canvase.remove();
            var image = document.createElement('img');
            image.src = canvase.toDataURL('/image.pmg');
            document.body.appendChild(image);
        };
        BackgroundImage.createImage = function () {
            var factor = caspian.common.getPixelsPerCentimetre() / 10;
            for (var index = 1; index <= 100; index++) {
                BackgroundImage.context.strokeStyle = '#999';
                var value = factor * index * 5;
                BackgroundImage.createHLine(value);
                BackgroundImage.createVLine(value);
            }
            for (var index = 1; index <= 50; index++) {
                BackgroundImage.context.strokeStyle = '#333';
                var value = factor * index * 10;
                BackgroundImage.createHLine(value);
                BackgroundImage.createVLine(value);
            }
        };
        BackgroundImage.createHLine = function (x) {
            BackgroundImage.context.beginPath();
            BackgroundImage.context.moveTo(x, 0);
            BackgroundImage.context.lineTo(x, 5000);
            BackgroundImage.context.stroke();
        };
        BackgroundImage.createVLine = function (y) {
            BackgroundImage.context.beginPath();
            BackgroundImage.context.moveTo(0, y);
            BackgroundImage.context.lineTo(5000, y);
            BackgroundImage.context.stroke();
        };
        return BackgroundImage;
    }());
    ReportGenerator.BackgroundImage = BackgroundImage;
})(ReportGenerator || (ReportGenerator = {}));
//# sourceMappingURL=ReportGenerator.js.map