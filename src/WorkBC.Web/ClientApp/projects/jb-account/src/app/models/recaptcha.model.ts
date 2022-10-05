
//export interface RecaptchaInput {
//    secret: string;
//    response: string;
//}

export interface RecaptchaResponse {
    success: boolean;
    challenge_ts: string;
    hostname: string;
}
