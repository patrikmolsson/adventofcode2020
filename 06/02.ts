const fs = require('fs');


(function main() {
    const groups = inputToGroupAnswers();

    let sum = 0;
    for (const group of groups) {
        const mapOfEveryYes = group.replace(/ /g, '').split('').reduce((acc, curr) => ({...acc, [curr]: true}), {} as Record<string, boolean>);
        const replies = group.split(' ');

        for (const reply of replies) {
            for (const key of Object.keys(mapOfEveryYes)) {
                if (!reply.includes(key)) {
                    delete mapOfEveryYes[key];
                }
            }
        }

        sum += Object.keys(mapOfEveryYes).length;
    }

    console.log(sum);
})();

function inputToGroupAnswers() {
    const lines = fs.readFileSync('./input.txt').toString('utf-8');

    return lines.split('\r\n\r\n').map(p => p.replace(/\r\n/g, ' '), '');
}