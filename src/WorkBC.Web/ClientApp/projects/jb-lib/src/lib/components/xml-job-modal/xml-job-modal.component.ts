import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-xml-job-modal',
  templateUrl: './xml-job-modal.component.html',
  styleUrls: ['./xml-job-modal.component.scss']
})
export class XmlJobModalComponent {

    public jobXml: string;
    public jobId: string;

  constructor(public activeModal: NgbActiveModal) {

    }

    createShiftArr(step: number | string) {
        let space = '';
        if (typeof step === 'number') { // argument is integer
            for (let i = 0; i < step; i++) {
                space += ' ';
            }
        } else { // argument is string
            space = step;
        }

        const shift = ['\n']; // array of shifts

        for (let ix = 0; ix < 100; ix++) {
            shift.push(shift[ix] + space);
        }

        return shift;
    }

    transform(xml: string, indent: number) {
        if (isFinite(indent)) {
            if (indent !== 0) {
                indent = indent || 2;
            }
        } else if (typeof indent !== 'string') {
            indent = 2;
        }

        const arr = xml.replace(/>\s*</gm, '><')
            .replace(/</g, '~::~<')
            .replace(/\s*xmlns([=:])/g, '~::~xmlns$1')
            .split('~::~');

        const len = arr.length;
        let inComment = false,
            depth = 0,
            string = '';
        const shift = this.createShiftArr(indent);

        for (let i = 0; i < len; i++) {
            // start comment or <![CDATA[...]]> or <!DOCTYPE //
            if (arr[i].indexOf('<!') !== -1) {
                string += shift[depth] + arr[i];
                inComment = true;

                // end comment or <![CDATA[...]]> //
                if (arr[i].indexOf('-->') !== -1 || arr[i].indexOf(']>') !== -1 ||
                    arr[i].indexOf('!DOCTYPE') !== -1) {
                    inComment = false;
                }
            } else if (arr[i].indexOf('-->') !== -1 || arr[i].indexOf(']>') !== -1) { // end comment  or <![CDATA[...]]> //
                string += arr[i];
                inComment = false;
            } else if (/^<\w/.test(arr[i - 1]) && /^<\/\w/.test(arr[i]) && // <elm></elm> //
                /^<[\w:\-.,]+/.exec(arr[i - 1])[0] === /^<\/[\w:\-.,]+/.exec(arr[i])[0].replace('/', '')) {
                string += arr[i];
                if (!inComment) {
                    depth--;
                }
            } else if (arr[i].search(/<\w/) !== -1 && arr[i].indexOf('</') === -1 && arr[i].indexOf('/>') === -1) { // <elm> //
                string += !inComment ? (shift[depth++] + arr[i]) : arr[i];
            } else if (arr[i].search(/<\w/) !== -1 && arr[i].indexOf('</') !== -1) { // <elm>...</elm> //
                string += !inComment ? shift[depth] + arr[i] : arr[i];
            } else if (arr[i].search(/<\//) > -1) { // </elm> //
                string += !inComment ? shift[--depth] + arr[i] : arr[i];
            } else if (arr[i].indexOf('/>') !== -1) { // <elm/> //
                string += !inComment ? shift[depth] + arr[i] : arr[i];
            } else if (arr[i].indexOf('<?') !== -1) { // <? xml ... ?> //
                string += shift[depth] + arr[i];
            } else if (arr[i].indexOf('xmlns:') !== -1 || arr[i].indexOf('xmlns=') !== -1) { // xmlns //
                string += shift[depth] + arr[i];
            } else {
                string += arr[i];
            }
        }

        console.log(string);

        return string.trim();
    }

}
