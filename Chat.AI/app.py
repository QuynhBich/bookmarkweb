import openai
import argparse
import os
import uvicorn
from fastapi import FastAPI, Path, Body
from fastapi.responses import JSONResponse
from starlette.responses import StreamingResponse

from dotenv import load_dotenv
load_dotenv()
openai_api_key = os.getenv("OPENAI_API_KEY")
app = FastAPI()
@app.post("/chat/{conversation_id}")
def chatbot(
    conversation_id: str = Path(..., title="Conversation ID"),
    link: str = Body(None, media_type="application/json", embed=True),
    question: str = Body(None, media_type="application/json", embed=True),
    expanded: bool = Body(False, media_type="application/json", embed=True),
):
    print(question)
    print(link)
    answer = ask_gpt(link, expanded,question)
    async def event_stream():
        yield answer # Your text data

    return StreamingResponse(event_stream(), media_type="text/event-stream")

def main():
    print("Starting GPT")
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "prompt", nargs="+", type=str, help="Prompt for GPT-3 to complete"
    )
    args = parser.parse_args()
    prompt = " ".join(args.prompt)
    answer = ask_gpt("https://en.wikipedia.org/wiki/Dog", False, "what is a dog")
    print(answer)

def ask_gpt(link: str, expanded: bool, question: str):
    openai.api_key = openai_api_key
    # expanded_text = (
    #     "Using both the information and also using your own knowledge.\n"
    #     if expanded
    #     else (
    #         "Just use the context information and do not use your own knowledge.\n"
    #         "If the context isn't helpful, simply state 'Text Not Found in this Page'.\n"
    #     )
    # )
    text_qa_template_str = (
        # f"{expanded_text}"

        # "Answer the question: what is a team"
        # "Following knowledge in this: https://asq.org/quality-resources/teams"
        # "if knowledge don't useful, simply state 'Answer not found in this link."
        # "If the answer is a multiple options, then show it as a list."

        # "Answer the question: "
        # f"{question} in this page: "+ link
        # " if you find the answer in content of this link: "+ link +", then state it"
        # # "else  simply state 'Answer not found in this link."
        # "Note: if the answer is a multiple options, then show it as a list."
        # "If the provided link does not contain a specific answer to the question, simply state 'Answer not found."
        "Answer the question: "
        f"{question} in this page: "+ link +
        " if you find the answer in content of that page, then state it"
        # "else  simply state 'Answer not found in this link."
        "Note: if the answer is a multiple options, then show it as a list."
        "If the provided link does not contain a specific answer to the question, simply state 'Answer not found."
    )
    response = openai.chat.completions.create(
    model="gpt-3.5-turbo",
    messages=[
        {"role": "user", "content": text_qa_template_str}
    ])
    content = response.choices[0].message.content
    return content

port = int(os.environ.get("PORT", 8000))
if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=port)
    # main()