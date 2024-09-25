namespace caspian {
    export class Accordion {
        el: HTMLElement;
        multiple: boolean;
        constructor(el: HTMLElement, multiple: boolean) {
            this.el = el;
            this.multiple = multiple || false;
            el
            //(el.querySelector('.submenu .selected') as HTMLElement).onclick = e => {
                   
            //    (e.target as HTMLElement).classList.remove('selected');
            //    //$(this).addClass('selected');
            //};

        }
    }
}