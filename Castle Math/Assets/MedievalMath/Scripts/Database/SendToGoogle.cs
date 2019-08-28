using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendToGoogle : MonoBehaviour
{
    string strand;
    string standard;
    string student;

    string question;
    string answer;
    string result;

    [SerializeField]
    private string BASE_URL = "https://docs.google.com/forms/d/e/1FAIpQLSf5z-8z6BgWcCWTMvGIYkqQvHnwvZjuHeY3_NfYd0iQJrTeSA/formResponse";
    public void Send()
    {
        student = "Jose";
        strand = "single digit";
        standard = "1.4.5";

        question = "1+3";
        answer = "4";
        result = "correct";

        StartCoroutine(Post(student, strand, standard, question, answer, result));
    }

    public void SendCustom(string cmd)
    {
        string[] cmds = cmd.Split(',');
        student = cmds[0];
        strand = cmds[1];
        standard = cmds[2];
        question = cmds[3];
        answer = cmds[4];
        result = cmds[5];

        StartCoroutine(Post(student, strand, standard, question, answer, result));
    }

    IEnumerator Post(string lStudent, string lStrand, string lStandard, string lQuestion, string lAnswer, string lResult)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.555690629", lStudent);

        form.AddField("entry.1613354179", lStrand);
        form.AddField("entry.1868823500", lStandard);
        form.AddField("entry.1371900724", lQuestion);
        form.AddField("entry.1593817650", lAnswer);
        form.AddField("entry.1583760873", lResult);

        byte[] rawData = form.data;

        WWW www = new WWW(BASE_URL, rawData);
        print(www);
        yield return www;
    }


}
