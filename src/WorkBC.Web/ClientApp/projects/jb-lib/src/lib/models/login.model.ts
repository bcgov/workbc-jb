import { AccountBase } from './user.model';

export interface Login extends AccountBase {
    password: string
}

export class LoginModel implements Login {
    username: string;

    constructor(
        public email: string = '',
        public password: string = ''
    ) { }
}
