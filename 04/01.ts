const fs = require('fs');

enum Tile {
    Square = 1,
    Tree
}

(function main() {
    const passports = inputToPassportLines();

    let validCount = 0;
    for (const passport of passports) {
        if (isValid(passport)) {
            console.log('passport is valid');
            validCount += 1;
            continue;
        }
        console.log('passport is invalid');
    }

    console.log(validCount);
})();

function isValid(passport: string) {
    const entries = passportToNumberOfEntries(passport);

    if (entries === 7 && passport.indexOf('cid:') === -1) {
        return true;
    } else if (entries === 8) {
        return true;
    }

    return false;
}

function passportToNumberOfEntries(passport: string) {
    const count = passport.split(' ').length;

    console.log('>>', passport, count);

    return count;
}

function inputToPassportLines() {
    const lines = fs.readFileSync('./input.txt').toString('utf-8');

    return lines.split('\r\n\r\n').map(p => p.replace(/\r\n/g, ' '));
}