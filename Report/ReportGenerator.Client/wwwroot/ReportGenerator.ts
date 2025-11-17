/// <reference path="../../../uicomponent/wwwroot/typescripts/caspian.common.ts" />
namespace ReportGenerator {
    export class BackgroundImage {
        private static context: CanvasRenderingContext2D;
        public static Initialize(): void {
            const canvase = document.createElement('canvas');
            //canvase.style.width = '10cm';
            //canvase.style.height = '10cm';
            canvase.width = caspian.common.getPixelsPerCentimetre() * 50;
            canvase.height = caspian.common.getPixelsPerCentimetre() * 50;
            document.body.appendChild(canvase);
            BackgroundImage.context = canvase.getContext('2d') as CanvasRenderingContext2D;
            BackgroundImage.createImage();
            canvase.remove();
            let image = document.createElement('img');
            image.src = canvase.toDataURL('/image.pmg');
            document.body.appendChild(image);
        }

        static createImage() {
            let factor = caspian.common.getPixelsPerCentimetre() / 10;
            for (let index = 1; index <= 100; index++) {
                BackgroundImage.context.strokeStyle = '#999';
                let value = factor * index * 5;
                BackgroundImage.createHLine(value);
                BackgroundImage.createVLine(value);
            }
            for (let index = 1; index <= 50; index++) {
                BackgroundImage.context.strokeStyle = '#333';
                let value = factor * index * 10;
                BackgroundImage.createHLine(value);
                BackgroundImage.createVLine(value);
            }
        }

        static createHLine(x: number) {
            BackgroundImage.context.beginPath();
            BackgroundImage.context.moveTo(x, 0);
            BackgroundImage.context.lineTo(x, 5000);
            BackgroundImage.context.stroke();
        }

        static createVLine(y:number) {
            BackgroundImage.context.beginPath();
            BackgroundImage.context.moveTo(0, y);
            BackgroundImage.context.lineTo(5000, y);
            BackgroundImage.context.stroke();
        }
    }
}



